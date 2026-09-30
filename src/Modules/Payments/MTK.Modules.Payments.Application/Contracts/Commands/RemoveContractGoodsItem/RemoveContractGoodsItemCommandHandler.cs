using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Contracts.Commands.RemoveContractGoodsItem;

internal sealed class RemoveContractGoodsItemCommandHandler : ICommandHandler<RemoveContractGoodsItemCommand>
{
    private readonly IContractRepository _contractRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveContractGoodsItemCommandHandler(
        IContractRepository contractRepository,
        IUnitOfWork unitOfWork)
    {
        _contractRepository = contractRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveContractGoodsItemCommand request, CancellationToken cancellationToken)
    {
        var contract = await _contractRepository.GetWithServicesAsync(request.ContractId, cancellationToken);

        if (contract is null)
        {
            return Result.Failure(new Error(
                "Contract.NotFound",
                $"Müqavilə tapılmadı: {request.ContractId}"));
        }

        contract.RemoveGoodsItem(request.GoodsItemId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
