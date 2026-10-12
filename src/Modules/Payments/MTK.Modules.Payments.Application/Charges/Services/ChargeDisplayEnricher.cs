using MTK.Modules.Payments.Application.Apartments.Services;
using MTK.Modules.Payments.Application.Buildings.Services;
using MTK.Modules.Payments.Application.Garages.Services;
using MTK.Modules.Payments.Application.Owners.Services;
using MTK.Modules.Payments.Application.Payments.Services;
using MTK.Modules.Payments.Application.Vendors.Services;
using MTK.Modules.Payments.Domain.Charges;

namespace MTK.Modules.Payments.Application.Charges.Services;

internal sealed class ChargeDisplayEnricher : IChargeDisplayEnricher
{
    private readonly ICachedOwnerService _cachedOwnerService;
    private readonly ICachedBuildingService _cachedBuildingService;
    private readonly ICachedVendorService _cachedVendorService;
    private readonly ICachedApartmentService _cachedApartmentService;
    private readonly ICachedGarageService _cachedGarageService;

    public ChargeDisplayEnricher(
        ICachedOwnerService cachedOwnerService,
        ICachedBuildingService cachedBuildingService,
        ICachedVendorService cachedVendorService,
        ICachedApartmentService cachedApartmentService,
        ICachedGarageService cachedGarageService)
    {
        _cachedOwnerService = cachedOwnerService;
        _cachedBuildingService = cachedBuildingService;
        _cachedVendorService = cachedVendorService;
        _cachedApartmentService = cachedApartmentService;
        _cachedGarageService = cachedGarageService;
    }

    public async Task<IReadOnlyDictionary<Guid, PartyPropertyDisplayInfo>> ResolveAsync(
        IReadOnlyCollection<Charge> charges,
        CancellationToken cancellationToken = default)
    {
        return await PartyPropertyDisplayResolver.ResolveAsync(
            charges,
            c => c.Id,
            c => c.OwnerId,
            c => c.VendorId,
            c => c.ApartmentId,
            c => c.GarageId,
            _cachedOwnerService,
            _cachedBuildingService,
            _cachedVendorService,
            _cachedApartmentService,
            _cachedGarageService,
            cancellationToken);
    }
}
