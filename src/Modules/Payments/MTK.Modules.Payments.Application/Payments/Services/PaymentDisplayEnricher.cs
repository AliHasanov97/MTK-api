using MTK.Modules.Payments.Application.Apartments.Services;
using MTK.Modules.Payments.Application.Buildings.Services;
using MTK.Modules.Payments.Application.Garages.Services;
using MTK.Modules.Payments.Application.Owners.Services;
using MTK.Modules.Payments.Application.Vendors.Services;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Parties;
using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Application.Payments.Services;

internal sealed class PaymentDisplayEnricher : IPaymentDisplayEnricher
{
    private readonly ICachedOwnerService _cachedOwnerService;
    private readonly ICachedBuildingService _cachedBuildingService;
    private readonly ICachedVendorService _cachedVendorService;
    private readonly ICachedApartmentService _cachedApartmentService;
    private readonly ICachedGarageService _cachedGarageService;

    public PaymentDisplayEnricher(
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
        IReadOnlyCollection<Payment> payments,
        CancellationToken cancellationToken = default)
    {
        return await PartyPropertyDisplayResolver.ResolveAsync(
            payments,
            p => p.Id,
            p => p.OwnerId,
            p => p.VendorId,
            p => p.ApartmentId,
            p => p.GarageId,
            _cachedOwnerService,
            _cachedBuildingService,
            _cachedVendorService,
            _cachedApartmentService,
            _cachedGarageService,
            cancellationToken);
    }
}
