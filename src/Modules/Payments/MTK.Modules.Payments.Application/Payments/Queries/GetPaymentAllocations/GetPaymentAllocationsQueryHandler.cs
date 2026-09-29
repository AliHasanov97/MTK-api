using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payments.Queries.GetPaymentAllocations;

internal sealed class GetPaymentAllocationsQueryHandler
    : IQueryHandler<GetPaymentAllocationsQuery, IReadOnlyCollection<PaymentAllocationDetailResponse>>
{
    private readonly IPaymentAllocationRepository _paymentAllocationRepository;
    private readonly IChargeRepository _chargeRepository;

    public GetPaymentAllocationsQueryHandler(
        IPaymentAllocationRepository paymentAllocationRepository,
        IChargeRepository chargeRepository)
    {
        _paymentAllocationRepository = paymentAllocationRepository;
        _chargeRepository = chargeRepository;
    }

    public async Task<Result<IReadOnlyCollection<PaymentAllocationDetailResponse>>> Handle(
        GetPaymentAllocationsQuery request,
        CancellationToken cancellationToken)
    {
        var allocations = await _paymentAllocationRepository.GetByPaymentIdAsync(request.PaymentId, cancellationToken);

        if (allocations.Count == 0)
        {
            return Result.Success<IReadOnlyCollection<PaymentAllocationDetailResponse>>(
                Array.Empty<PaymentAllocationDetailResponse>());
        }

        var chargeIds = allocations.Select(a => a.ChargeId).Distinct().ToList();
        var charges = await _chargeRepository.ListFromIdsAsync(chargeIds, cancellationToken);
        var chargesById = charges.ToDictionary(c => c.Id);

        var response = allocations
            .Where(a => chargesById.ContainsKey(a.ChargeId))
            .Select(a =>
            {
                var charge = chargesById[a.ChargeId];
                return new PaymentAllocationDetailResponse(
                    charge.Id,
                    charge.PropertyType,
                    charge.PropertyId,
                    charge.Period,
                    charge.Description,
                    charge.Amount,
                    a.Amount);
            })
            .ToList();

        return response;
    }
}
