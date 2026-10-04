using MTK.Modules.Payments.Application.Payments.Services;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Charges.Services;

internal sealed class ChargeDisplayEnricher : IChargeDisplayEnricher
{
    private readonly IOwnerRepository _ownerRepository;
    private readonly IVendorRepository _vendorRepository;
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IGarageRepository _garageRepository;

    public ChargeDisplayEnricher(
        IOwnerRepository ownerRepository,
        IVendorRepository vendorRepository,
        IApartmentRepository apartmentRepository,
        IGarageRepository garageRepository)
    {
        _ownerRepository = ownerRepository;
        _vendorRepository = vendorRepository;
        _apartmentRepository = apartmentRepository;
        _garageRepository = garageRepository;
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
            _ownerRepository,
            _vendorRepository,
            _apartmentRepository,
            _garageRepository,
            cancellationToken);
    }
}
