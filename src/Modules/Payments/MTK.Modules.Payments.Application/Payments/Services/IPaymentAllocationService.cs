using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Application.Payments.Services;

public interface IPaymentAllocationService
{
    /// <summary>
    /// Allocates a payment FIFO against the owner's oldest unpaid charges. When
    /// <paramref name="propertyId"/> is given, only that property's charges are eligible —
    /// the payment will not spill over onto the owner's other apartments/garages.
    /// </summary>
    Task<Result> AllocatePaymentAsync(Guid paymentId, Guid ownerId, decimal amount, Guid? propertyId, CancellationToken cancellationToken);

    /// <summary>
    /// The mirror operation, run whenever a new charge is created (monthly generation or
    /// a manual one-off): pays it down from whatever's left over from the owner's own
    /// past payments (oldest first), regardless of which property they originally
    /// targeted — an apartment/garage never carries its own advance (see
    /// PropertyBalanceResponse), so any surplus sitting on an owner's older payments is
    /// exactly what a brand-new charge should be settled from first.
    /// </summary>
    Task<Result> SettleChargeFromAdvanceAsync(Guid chargeId, Guid ownerId, CancellationToken cancellationToken);
}
