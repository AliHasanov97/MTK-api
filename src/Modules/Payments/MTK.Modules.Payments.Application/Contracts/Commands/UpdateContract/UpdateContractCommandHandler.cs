using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Contracts.Commands.UpdateContract;

internal sealed class UpdateContractCommandHandler : ICommandHandler<UpdateContractCommand>
{
    private readonly IContractRepository _contractRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateContractCommandHandler(
        IContractRepository contractRepository,
        IUnitOfWork unitOfWork)
    {
        _contractRepository = contractRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateContractCommand request, CancellationToken cancellationToken)
    {
        var contract = await _contractRepository.GetByIdAsync(request.ContractId, cancellationToken);

        if (contract is null)
        {
            return Result.Failure(new Error(
                "Contract.NotFound",
                $"Müqavilə tapılmadı: {request.ContractId}"));
        }

        contract.UpdateDates(request.StartDate, request.EndDate, request.Note);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
