using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Vendors.Commands.UpdateVendor;

internal sealed class UpdateVendorCommandHandler : ICommandHandler<UpdateVendorCommand>
{
    private readonly IVendorRepository _vendorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateVendorCommandHandler(
        IVendorRepository vendorRepository,
        IUnitOfWork unitOfWork)
    {
        _vendorRepository = vendorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateVendorCommand request, CancellationToken cancellationToken)
    {
        var vendor = await _vendorRepository.GetByIdAsync(request.VendorId, cancellationToken);

        if (vendor is null)
        {
            return Result.Failure(new Error("Vendor.NotFound", $"Tədarükçü tapılmadı: {request.VendorId}"));
        }

        if (!string.IsNullOrWhiteSpace(request.Voen) &&
            !await _vendorRepository.IsVoenUniqueAsync(request.Voen, request.VendorId, cancellationToken))
        {
            return Result.Failure(new Error(
                "Vendor.VoenAlreadyExists",
                $"Bu VÖEN ilə başqa tədarükçü mövcuddur: {request.Voen}"));
        }

        vendor.UpdateIdentity(request.Name, request.VendorType, request.Voen);
        vendor.UpdateContactInfo(
            request.Director,
            request.Email,
            request.Phone,
            request.Address,
            request.Note);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
