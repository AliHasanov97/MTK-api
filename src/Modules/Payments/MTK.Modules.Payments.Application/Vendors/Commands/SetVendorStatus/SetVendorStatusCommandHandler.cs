using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Vendors.Commands.SetVendorStatus;

internal sealed class SetVendorStatusCommandHandler : ICommandHandler<SetVendorStatusCommand>
{
    private readonly IVendorRepository _vendorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SetVendorStatusCommandHandler(
        IVendorRepository vendorRepository,
        IUnitOfWork unitOfWork)
    {
        _vendorRepository = vendorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SetVendorStatusCommand request, CancellationToken cancellationToken)
    {
        var vendor = await _vendorRepository.GetByIdAsync(request.VendorId, cancellationToken);

        if (vendor is null)
        {
            return Result.Failure(new Error("Vendor.NotFound", $"Tədarükçü tapılmadı: {request.VendorId}"));
        }

        if (request.IsActive)
        {
            vendor.Activate();
        }
        else
        {
            vendor.Deactivate();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
