using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Contracts.Commands.UpdateContractService;

internal sealed class UpdateContractServiceCommandHandler : ICommandHandler<UpdateContractServiceCommand>
{
    private readonly IContractRepository _contractRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateContractServiceCommandHandler(
        IContractRepository contractRepository,
        IUnitOfWork unitOfWork)
    {
        _contractRepository = contractRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateContractServiceCommand request, CancellationToken cancellationToken)
    {
        var contract = await _contractRepository.GetWithServicesAsync(request.ContractId, cancellationToken);

        if (contract is null)
        {
            return Result.Failure(new Error(
                "Contract.NotFound",
                $"Müqavilə tapılmadı: {request.ContractId}"));
        }

        contract.UpdateService(
            request.ServiceId,
            request.Name,
            request.UnitPrice,
            request.BillingPeriod,
            request.Description,
            request.ServiceStartDate,
            request.ServiceEndDate,
            request.PaymentTermDays);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
