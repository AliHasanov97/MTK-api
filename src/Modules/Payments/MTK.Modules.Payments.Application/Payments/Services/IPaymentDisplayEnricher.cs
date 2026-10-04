using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Application.Payments.Services;

/// <summary>
/// Resolves owner/vendor names and apartment/garage labels for a batch of payments, from
/// Payments' own Owner/Apartment/Garage shadows — no cross-module call, no frontend
/// round-trip to Buildings/Identity needed just to show "kimin ödənişi, hansı mənzil".
/// Batches lookups by distinct id to avoid N+1 across a payment list. See
/// MTK.Modules.Payments.Application.Charges.Services.IChargeDisplayEnricher for the
/// Charge-side equivalent — both share PartyPropertyDisplayResolver's lookup logic.
/// </summary>
public interface IPaymentDisplayEnricher
{
    Task<IReadOnlyDictionary<Guid, PartyPropertyDisplayInfo>> ResolveAsync(
        IReadOnlyCollection<Payment> payments,
        CancellationToken cancellationToken = default);
}
