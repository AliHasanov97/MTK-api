using MTK.Modules.Payments.Application.Owners.Services;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Parties;
using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payments.Services;

internal sealed class PaymentDisplayEnricher : IPaymentDisplayEnricher
{
    private readonly ICachedOwnerService _cachedOwnerService;
    private readonly IVendorRepository _vendorRepository;
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IGarageRepository _garageRepository;

    public PaymentDisplayEnricher(
        ICachedOwnerService cachedOwnerService,
        IVendorRepository vendorRepository,
        IApartmentRepository apartmentRepository,
        IGarageRepository garageRepository)
    {
        _cachedOwnerService = cachedOwnerService;
        _vendorRepository = vendorRepository;
        _apartmentRepository = apartmentRepository;
        _garageRepository = garageRepository;
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
            _vendorRepository,
            _apartmentRepository,
            _garageRepository,
            cancellationToken);
    }
}
