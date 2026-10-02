using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Charges.Queries.GetChargeAllocations;

internal sealed class GetChargeAllocationsQueryHandler
    : IQueryHandler<GetChargeAllocationsQuery, IReadOnlyCollection<ChargeAllocationResponse>>
{
    private readonly IPaymentAllocationRepository _paymentAllocationRepository;
    private readonly IPaymentRepository _paymentRepository;

    public GetChargeAllocationsQueryHandler(
        IPaymentAllocationRepository paymentAllocationRepository,
        IPaymentRepository paymentRepository)
    {
        _paymentAllocationRepository = paymentAllocationRepository;
        _paymentRepository = paymentRepository;
    }

    public async Task<Result<IReadOnlyCollection<ChargeAllocationResponse>>> Handle(
        GetChargeAllocationsQuery request,
        CancellationToken cancellationToken)
    {
        var allocations = await _paymentAllocationRepository.GetByChargeIdAsync(request.ChargeId, cancellationToken);

        if (allocations.Count == 0)
        {
            return Result.Success<IReadOnlyCollection<ChargeAllocationResponse>>(Array.Empty<ChargeAllocationResponse>());
        }

        var paymentIds = allocations.Select(a => a.PaymentId).Distinct().ToList();
        var payments = await _paymentRepository.ListFromIdsAsync(paymentIds, cancellationToken);
        var paymentsById = payments.ToDictionary(p => p.Id);

        var response = allocations
            .Where(a => paymentsById.ContainsKey(a.PaymentId))
            .Select(a =>
            {
                var payment = paymentsById[a.PaymentId];
                return new ChargeAllocationResponse(
                    payment.Id,
                    a.Amount,
                    payment.PaymentDate,
                    payment.Status.ToString(),
                    a.RemainingDebtAfterPayment,
                    a.IsFromAdvance);
            })
            .OrderBy(a => a.PaymentDate)
            .ToList();

        return response;
    }
}
