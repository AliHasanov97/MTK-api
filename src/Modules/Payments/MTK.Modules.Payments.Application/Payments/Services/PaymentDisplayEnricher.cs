using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Parties;
using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payments.Services;

internal sealed class PaymentDisplayEnricher : IPaymentDisplayEnricher
{
    private readonly IOwnerRepository _ownerRepository;
    private readonly IVendorRepository _vendorRepository;
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IGarageRepository _garageRepository;

    public PaymentDisplayEnricher(
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
            _ownerRepository,
            _vendorRepository,
            _apartmentRepository,
            _garageRepository,
            cancellationToken);
    }
}
