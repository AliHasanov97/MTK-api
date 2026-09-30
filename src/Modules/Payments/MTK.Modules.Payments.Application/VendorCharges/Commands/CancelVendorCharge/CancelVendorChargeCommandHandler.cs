using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.VendorCharges.Commands.CancelVendorCharge;

internal sealed class CancelVendorChargeCommandHandler : ICommandHandler<CancelVendorChargeCommand>
{
    private readonly IVendorChargeRepository _vendorChargeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelVendorChargeCommandHandler(
        IVendorChargeRepository vendorChargeRepository,
        IUnitOfWork unitOfWork)
    {
        _vendorChargeRepository = vendorChargeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CancelVendorChargeCommand request, CancellationToken cancellationToken)
    {
        var charge = await _vendorChargeRepository.GetByIdAsync(request.ChargeId, cancellationToken);

        if (charge is null)
        {
            return Result.Failure(new Error(
                "VendorCharge.NotFound",
                $"Tədarükçü borcu tapılmadı: {request.ChargeId}"));
        }

        try
        {
            charge.Cancel(request.Reason);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(new Error("VendorCharge.CancelNotAllowed", ex.Message));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
