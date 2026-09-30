using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Vendors.Commands.DeleteVendor;

internal sealed class DeleteVendorCommandHandler : ICommandHandler<DeleteVendorCommand>
{
    private readonly IVendorRepository _vendorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteVendorCommandHandler(
        IVendorRepository vendorRepository,
        IUnitOfWork unitOfWork)
    {
        _vendorRepository = vendorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteVendorCommand request, CancellationToken cancellationToken)
    {
        var vendor = await _vendorRepository.GetByIdAsync(request.VendorId, cancellationToken);

        if (vendor is null)
        {
            return Result.Failure(new Error("Vendor.NotFound", $"Tədarükçü tapılmadı: {request.VendorId}"));
        }

        if (vendor.DeletedAt is not null)
        {
            return Result.Failure(new Error("Vendor.AlreadyDeleted", "Tədarükçü artıq silinib"));
        }

        // Aktiv müqaviləsi olan tədarükçü silinmir — əvvəlcə müqavilə ləğv edilməlidir.
        if (await _vendorRepository.HasActiveContractsAsync(request.VendorId, cancellationToken))
        {
            return Result.Failure(new Error(
                "Vendor.HasActiveContracts",
                "Bu tədarükçünün aktiv müqaviləsi var. Əvvəlcə müqaviləni ləğv edin və ya tədarükçünü dayandırın"));
        }

        vendor.Delete();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
