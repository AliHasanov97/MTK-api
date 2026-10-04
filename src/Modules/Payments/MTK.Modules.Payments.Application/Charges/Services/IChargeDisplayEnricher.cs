using MTK.Modules.Payments.Application.Payments.Services;
using MTK.Modules.Payments.Domain.Charges;

namespace MTK.Modules.Payments.Application.Charges.Services;

/// <summary>
/// Charge-side equivalent of IPaymentDisplayEnricher — same (OwnerId, VendorId,
/// ApartmentId, GarageId) shape as Payment, so it shares PartyPropertyDisplayResolver's
/// Owner/Vendor/Apartment/Garage shadow lookups.
/// </summary>
public interface IChargeDisplayEnricher
{
    Task<IReadOnlyDictionary<Guid, PartyPropertyDisplayInfo>> ResolveAsync(
        IReadOnlyCollection<Charge> charges,
        CancellationToken cancellationToken = default);
}
