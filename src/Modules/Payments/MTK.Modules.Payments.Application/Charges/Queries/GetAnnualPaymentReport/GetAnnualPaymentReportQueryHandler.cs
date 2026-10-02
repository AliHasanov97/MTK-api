using System.Text.RegularExpressions;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Charges.Queries.GetAnnualPaymentReport;

internal sealed partial class GetAnnualPaymentReportQueryHandler
    : IQueryHandler<GetAnnualPaymentReportQuery, AnnualPaymentReportResponse>
{
    private readonly IChargeRepository _chargeRepository;
    private readonly IPropertyOwnershipRepository _propertyOwnershipRepository;

    public GetAnnualPaymentReportQueryHandler(
        IChargeRepository chargeRepository,
        IPropertyOwnershipRepository propertyOwnershipRepository)
    {
        _chargeRepository = chargeRepository;
        _propertyOwnershipRepository = propertyOwnershipRepository;
    }

    public async Task<Result<AnnualPaymentReportResponse>> Handle(
        GetAnnualPaymentReportQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Year is < 2000 or > 2200)
        {
            return Result.Failure<AnnualPaymentReportResponse>(new Error(
                "AnnualPaymentReport.InvalidYear",
                "İl 2000-2200 aralığında olmalıdır"));
        }

        var properties = request.PropertyType is { } propertyType
            ? await _propertyOwnershipRepository.GetByPropertyTypeAsync(propertyType, cancellationToken)
            : await _propertyOwnershipRepository.GetAllWithOwnersAsync(cancellationToken);

        var charges = await _chargeRepository.GetOwnerChargesByYearAsync(request.Year, cancellationToken);
        var outstandingByProperty = await _chargeRepository.GetOutstandingAmountByPropertyIdsAsync(
            properties.Select(p => p.PropertyId).ToList(), cancellationToken);

        // Period-u tam "yyyy-MM" formatına uyğun olanlar — manual borclar fərqli bir
        // teq daşıya bilər (məs. "MANUAL-...") və bu hesabatda nəzərə alınmır.
        var chargesByPropertyAndMonth = charges
            .Where(c => c.PropertyId is not null && PeriodPattern().IsMatch(c.Period!))
            .GroupBy(c => c.PropertyId!.Value)
            .ToDictionary(
                g => g.Key,
                g => g.GroupBy(c => int.Parse(c.Period![5..7])).ToDictionary(mg => mg.Key, mg => mg.ToList()));

        var rows = properties
            .Select(ownership =>
            {
                chargesByPropertyAndMonth.TryGetValue(ownership.PropertyId, out var byMonth);

                var months = Enumerable.Range(1, 12)
                    .Select(month =>
                    {
                        var monthCharges = byMonth is not null && byMonth.TryGetValue(month, out var list)
                            ? list.Where(c => c.Status != ChargeStatus.Cancelled).ToList()
                            : [];

                        if (monthCharges.Count == 0)
                        {
                            return new MonthlyChargeSummary(month, 0, 0, Status: null);
                        }

                        var amount = monthCharges.Sum(c => c.Amount);
                        var paid = monthCharges.Sum(c => c.PaidAmount);
                        var status = paid >= amount
                            ? ChargeStatus.Paid
                            : paid > 0
                                ? ChargeStatus.PartiallyPaid
                                : ChargeStatus.Unpaid;

                        return new MonthlyChargeSummary(month, amount, paid, status);
                    })
                    .ToList();

                return new PropertyAnnualReportRow(
                    ownership.PropertyId,
                    ownership.PropertyType,
                    months,
                    outstandingByProperty.GetValueOrDefault(ownership.PropertyId));
            })
            .OrderBy(r => r.PropertyType)
            .ThenBy(r => r.PropertyId)
            .ToList();

        return new AnnualPaymentReportResponse(request.Year, rows);
    }

    [GeneratedRegex(@"^\d{4}-(0[1-9]|1[0-2])$")]
    private static partial Regex PeriodPattern();
}
