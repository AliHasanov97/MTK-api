using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.Vendors.Services;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Domain.Vendors;

namespace MTK.Modules.Payments.Application.Vendors.Commands.CreateVendor;

internal sealed class CreateVendorCommandHandler : ICommandHandler<CreateVendorCommand, Guid>
{
    private readonly IVendorRepository _vendorRepository;
    private readonly ICachedVendorService _cachedVendorService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateVendorCommandHandler(
        IVendorRepository vendorRepository,
        ICachedVendorService cachedVendorService,
        IUnitOfWork unitOfWork)
    {
        _vendorRepository = vendorRepository;
        _cachedVendorService = cachedVendorService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateVendorCommand request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.Voen) &&
            !await _vendorRepository.IsVoenUniqueAsync(request.Voen, null, cancellationToken))
        {
            return Result.Failure<Guid>(new Error(
                "Vendor.VoenAlreadyExists",
                $"Bu VÖEN ilə tədarükçü artıq mövcuddur: {request.Voen}"));
        }

        var vendor = Vendor.Create(
            request.Name,
            request.VendorType,
            request.Voen,
            request.Director,
            request.Email,
            request.Phone,
            request.Address,
            request.Note);

        _vendorRepository.Add(vendor);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate cache (precautionary, vendor is new so unlikely to be cached)
        await _cachedVendorService.InvalidateCacheAsync(vendor.Id, cancellationToken);

        return Result.Success(vendor.Id);
    }
}
