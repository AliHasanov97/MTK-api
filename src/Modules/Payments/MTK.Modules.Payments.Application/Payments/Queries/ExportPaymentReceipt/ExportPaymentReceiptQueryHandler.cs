using MTK.Common.Application.Authorization;
using MTK.Common.Application.Exporting;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Services.Export;
using MTK.Modules.Payments.Domain.Apartments;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Garages;
using MTK.Modules.Payments.Domain.Parties;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payments.Queries.ExportPaymentReceipt;

internal sealed class ExportPaymentReceiptQueryHandler : IQueryHandler<ExportPaymentReceiptQuery, ExportFileResult>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentAllocationRepository _paymentAllocationRepository;
    private readonly IChargeRepository _chargeRepository;
    private readonly IOwnerRepository _ownerRepository;
    private readonly IVendorRepository _vendorRepository;
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IGarageRepository _garageRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IPaymentReceiptExportService _exportService;

    public ExportPaymentReceiptQueryHandler(
        IPaymentRepository paymentRepository,
        IPaymentAllocationRepository paymentAllocationRepository,
        IChargeRepository chargeRepository,
        IOwnerRepository ownerRepository,
        IVendorRepository vendorRepository,
        IApartmentRepository apartmentRepository,
        IGarageRepository garageRepository,
        IAuditLogRepository auditLogRepository,
        IUserRepository userRepository,
        ICompanyRepository companyRepository,
        IPaymentReceiptExportService exportService)
    {
        _paymentRepository = paymentRepository;
        _paymentAllocationRepository = paymentAllocationRepository;
        _chargeRepository = chargeRepository;
        _ownerRepository = ownerRepository;
        _vendorRepository = vendorRepository;
        _apartmentRepository = apartmentRepository;
        _garageRepository = garageRepository;
        _auditLogRepository = auditLogRepository;
        _userRepository = userRepository;
        _companyRepository = companyRepository;
        _exportService = exportService;
    }

    private static readonly Dictionary<string, string> RoleTitles = new()
    {
        [Roles.BuildingManager] = "Komendantı",
        [Roles.Accountant] = "Xəzinədarı",
        [Roles.Admin] = "Rəhbərliyi",
    };

    public async Task<Result<ExportFileResult>> Handle(
        ExportPaymentReceiptQuery request,
        CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdDefaultAsync(request.PaymentId, cancellationToken);
        if (payment is null)
        {
            return Result.Failure<ExportFileResult>(new Error(
                "Payment.NotFound",
                $"Ödəniş tapılmadı: {request.PaymentId}"));
        }

        var allocations = await _paymentAllocationRepository.GetByPaymentIdAsync(payment.Id, cancellationToken);
        var chargeIds = allocations.Select(a => a.ChargeId).Distinct().ToList();
        var charges = await _chargeRepository.ListFromIdsAsync(chargeIds, cancellationToken);
        var chargesById = charges.ToDictionary(c => c.Id);

        // Each applied line carries its own unit and building. A general-account payment
        // can be allocated across several properties, so the receipt must show the actual
        // destination for each applied amount rather than infer one destination for the
        // whole payment.
        var apartmentIds = charges.Where(c => c.ApartmentId.HasValue).Select(c => c.ApartmentId!.Value).Distinct().ToList();
        var garageIds = charges.Where(c => c.GarageId.HasValue).Select(c => c.GarageId!.Value).Distinct().ToList();

        var apartmentsById = apartmentIds.Count > 0
            ? (await _apartmentRepository.ListFromIdsWithBuildingAsync(apartmentIds, cancellationToken)).ToDictionary(a => a.Id)
            : new Dictionary<Guid, Apartment>();
        var garagesById = garageIds.Count > 0
            ? (await _garageRepository.ListFromIdsAsync(garageIds, cancellationToken)).ToDictionary(g => g.Id)
            : new Dictionary<Guid, Garage>();

        var lines = allocations
            .Select(a =>
            {
                chargesById.TryGetValue(a.ChargeId, out var charge);
                // Monthly auto-generated charges carry no free-text Description (only
                // manual/vendor charges do) — without a period fallback here, every line
                // of a bulk payment covering several months would show as plain "Haqq"
                // with no way to tell which month each amount belongs to.
                string description = charge?.Description ?? "Kommunal haqqı";

                string? propertyLabel = charge?.ApartmentId is { } aptId && apartmentsById.TryGetValue(aptId, out var apt)
                    ? $"Mənzil № {apt.ApartmentNumber} · {apt.Building.Name} binası"
                    : charge?.GarageId is { } garId && garagesById.TryGetValue(garId, out var gar)
                        ? $"Qaraj № {gar.GarageNumber}"
                        : null;

                return new PaymentReceiptLine(description, a.Amount, charge?.Period, propertyLabel);
            })
            .ToList();

        // The receipt's "for [month] [year]" line only makes sense when every charge
        // this payment settled shares one billing period — leave it null otherwise
        // (several months, or a pure advance with no charge behind it) and let the PDF
        // fall back to the payment's own date.
        var distinctPeriods = charges
            .Select(c => c.Period)
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Distinct()
            .ToList();
        string? period = distinctPeriods.Count == 1 ? distinctPeriods[0] : null;

        var owner = payment.PartyType == PartyType.Owner
            ? await _ownerRepository.GetByIdDefaultAsync(payment.PartyId, cancellationToken)
            : null;
        string? payerName = payment.PartyType == PartyType.Owner
            ? owner?.FullName
            : (await _vendorRepository.GetByIdDefaultAsync(payment.PartyId, cancellationToken))?.Name;
        string? payerEmail = owner?.Email;

        var creationAudit = await _auditLogRepository.GetForEntityActionAsync(
            "Payment",
            payment.Id,
            "Created",
            cancellationToken);

        string? issuerName = null;
        string? issuerTitle = null;
        if (creationAudit?.UserId is { } issuerUserId)
        {
            issuerName = (await _userRepository.GetByIdDefaultAsync(issuerUserId, cancellationToken))?.FullName;
        }
        if (creationAudit?.ActorRole is { } actorRole)
        {
            RoleTitles.TryGetValue(actorRole, out issuerTitle);
        }

        var company = await _companyRepository.GetAsync(cancellationToken);

        var data = new PaymentReceiptData(
            payment.Id,
            payment.ReceiptNumber,
            company?.Name ?? "MTK",
            payment.Amount,
            payment.PaymentMethod,
            payment.PaymentDate,
            payment.Notes,
            payment.PartyType,
            payerName,
            period,
            lines,
            issuerName,
            issuerTitle,
            PayerEmail: payerEmail);

        var stream = _exportService.ExportToPdf(data);
        var fileName = $"Qebz_{payment.ReceiptNumber:D8}.pdf";

        return new ExportFileResult(stream.ToArray(), fileName, "application/pdf");
    }
}
