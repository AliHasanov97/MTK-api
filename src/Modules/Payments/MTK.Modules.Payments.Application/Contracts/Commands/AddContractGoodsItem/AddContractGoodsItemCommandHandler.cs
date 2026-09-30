using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Contracts.Commands.AddContractGoodsItem;

internal sealed class AddContractGoodsItemCommandHandler : ICommandHandler<AddContractGoodsItemCommand, Guid>
{
    private readonly IContractRepository _contractRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddContractGoodsItemCommandHandler(
        IContractRepository contractRepository,
        IUnitOfWork unitOfWork)
    {
        _contractRepository = contractRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(AddContractGoodsItemCommand request, CancellationToken cancellationToken)
    {
        var contract = await _contractRepository.GetWithServicesAsync(request.ContractId, cancellationToken);

        if (contract is null)
        {
            return Result.Failure<Guid>(new Error(
                "Contract.NotFound",
                $"Müqavilə tapılmadı: {request.ContractId}"));
        }

        var item = contract.AddGoodsItem(
            request.Name,
            request.Unit,
            request.UnitPrice,
            request.AgreedQuantity,
            request.PaymentTermDays,
            request.Description);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(item.Id);
    }
}
