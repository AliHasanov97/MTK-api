using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.VendorCharges.Commands.RecordGoodsDelivery;

internal sealed class RecordGoodsDeliveryCommandHandler : ICommandHandler<RecordGoodsDeliveryCommand, Guid>
{
    private readonly IContractRepository _contractRepository;
    private readonly IVendorChargeRepository _vendorChargeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RecordGoodsDeliveryCommandHandler(
        IContractRepository contractRepository,
        IVendorChargeRepository vendorChargeRepository,
        IUnitOfWork unitOfWork)
    {
        _contractRepository = contractRepository;
        _vendorChargeRepository = vendorChargeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(RecordGoodsDeliveryCommand request, CancellationToken cancellationToken)
    {
        var contract = await _contractRepository.GetWithServicesAsync(request.ContractId, cancellationToken);

        if (contract is null)
        {
            return Result.Failure<Guid>(new Error(
                "Contract.NotFound",
                $"Müqavilə tapılmadı: {request.ContractId}"));
        }

        var item = contract.GoodsItems.FirstOrDefault(i => i.Id == request.GoodsItemId);

        if (item is null)
        {
            return Result.Failure<Guid>(new Error(
                "Contract.GoodsItemNotFound",
                "Mal sətri bu müqavilədə tapılmadı"));
        }

        if (!item.IsActive)
        {
            return Result.Failure<Guid>(new Error(
                "Contract.GoodsItemInactive",
                "Dayandırılmış mal sətri üzrə tədarük qeydə alına bilməz"));
        }

        if (contract.Status != Domain.Contracts.ContractStatus.Active)
        {
            return Result.Failure<Guid>(new Error(
                "Contract.NotActive",
                "Tədarük yalnız aktiv müqavilə üzrə qeydə alına bilər"));
        }

        if (request.Quantity <= 0)
        {
            return Result.Failure<Guid>(new Error(
                "VendorCharge.InvalidQuantity",
                "Tədarük miqdarı müsbət olmalıdır"));
        }

        if (!string.IsNullOrWhiteSpace(request.Reference))
        {
            bool alreadyRecorded = await _vendorChargeRepository.ExistsForReferenceAsync(
                item.Id,
                request.Reference!,
                cancellationToken);

            if (alreadyRecorded)
            {
                return Result.Failure<Guid>(new Error(
                    "VendorCharge.DuplicateReference",
                    $"Bu qaimə nömrəsi artıq qeydə alınıb: {request.Reference}"));
            }
        }

        var charge = Domain.VendorCharges.VendorCharge.ForGoodsDelivery(
            contract,
            item,
            request.Quantity,
            request.Reference,
            request.DeliveredOn ?? DateTimeOffset.UtcNow);

        _vendorChargeRepository.Add(charge);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(charge.Id);
    }
}
