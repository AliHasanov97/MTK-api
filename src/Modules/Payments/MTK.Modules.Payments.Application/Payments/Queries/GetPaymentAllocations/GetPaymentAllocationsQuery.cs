using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Domain.Charges;

namespace MTK.Modules.Payments.Application.Payments.Queries.GetPaymentAllocations;

public sealed record GetPaymentAllocationsQuery(Guid PaymentId) : IQuery<IReadOnlyCollection<PaymentAllocationDetailResponse>>;

/// <summary>
/// One line of "this payment covered this charge, for this much" — lets the UI
/// answer "what did this 60 AZN actually pay for?" when a payment is expanded.
/// </summary>
public sealed record PaymentAllocationDetailResponse(
    Guid ChargeId,
    PropertyType PropertyType,
    Guid PropertyId,
    string Period,
    string? Description,
    decimal ChargeAmount,
    decimal AllocatedAmount);
