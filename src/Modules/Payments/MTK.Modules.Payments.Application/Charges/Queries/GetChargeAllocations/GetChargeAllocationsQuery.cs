using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Charges.Queries.GetChargeAllocations;

public sealed record GetChargeAllocationsQuery(Guid ChargeId) : IQuery<IReadOnlyCollection<ChargeAllocationResponse>>;

public sealed record ChargeAllocationResponse(
    Guid PaymentId,
    decimal AllocatedAmount,
    DateTimeOffset PaymentDate,
    string PaymentMethod,
    string PaymentStatus,
    string? Reference);
