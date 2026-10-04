using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Payments.Queries.GetPaymentAllocations;

public sealed record GetPaymentAllocationsQuery(Guid PaymentId) : IQuery<IReadOnlyCollection<PaymentAllocationDetailResponse>>;

/// <summary>
/// One line of "this payment covered this charge, for this much" — lets the UI
/// answer "what did this 60 AZN actually pay for?" when a payment is expanded.
///
/// <see cref="RemainingDebtAfterPayment"/> is this specific charge's own remaining
/// balance right after this allocation was applied — see
/// <see cref="Payments.PaymentAllocation.RemainingDebtAfterPayment"/>.
/// </summary>
public sealed record PaymentAllocationDetailResponse(
    // PaymentAllocation-un öz Id-si — AuditLogs-da "kim icra edib"-i axtarmaq üçün.
    Guid Id,
    Guid ChargeId,
    Guid? ApartmentId,
    Guid? GarageId,
    string? Period,
    string? Description,
    decimal ChargeAmount,
    decimal AllocatedAmount,
    decimal RemainingDebtAfterPayment,
    // Resolved server-side from the Apartment/Garage shadow — "Mənzil N — Bina"/"Qaraj
    // N" — null when both ApartmentId/GarageId are null (a general/advance allocation)
    // or the shadow hasn't synced yet.
    string? PropertyLabel = null);
