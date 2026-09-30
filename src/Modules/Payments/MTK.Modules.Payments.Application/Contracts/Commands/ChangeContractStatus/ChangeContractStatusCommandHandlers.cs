using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Contracts;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Contracts.Commands.ChangeContractStatus;

internal sealed class ActivateContractCommandHandler : ICommandHandler<ActivateContractCommand>
{
    private readonly IContractRepository _contractRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateContractCommandHandler(
        IContractRepository contractRepository,
        IUnitOfWork unitOfWork)
    {
        _contractRepository = contractRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ActivateContractCommand request, CancellationToken cancellationToken)
    {
        var contract = await _contractRepository.GetWithServicesAsync(request.ContractId, cancellationToken);

        if (contract is null)
        {
            return Result.Failure(NotFound(request.ContractId));
        }

        contract.Activate();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static Error NotFound(Guid contractId) =>
        new("Contract.NotFound", $"Müqavilə tapılmadı: {contractId}");
}

internal sealed class SuspendContractCommandHandler : ICommandHandler<SuspendContractCommand>
{
    private readonly IContractRepository _contractRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SuspendContractCommandHandler(
        IContractRepository contractRepository,
        IUnitOfWork unitOfWork)
    {
        _contractRepository = contractRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SuspendContractCommand request, CancellationToken cancellationToken)
    {
        var contract = await _contractRepository.GetByIdAsync(request.ContractId, cancellationToken);

        if (contract is null)
        {
            return Result.Failure(new Error("Contract.NotFound", $"Müqavilə tapılmadı: {request.ContractId}"));
        }

        contract.Suspend(request.Note);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

internal sealed class TerminateContractCommandHandler : ICommandHandler<TerminateContractCommand>
{
    private readonly IContractRepository _contractRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TerminateContractCommandHandler(
        IContractRepository contractRepository,
        IUnitOfWork unitOfWork)
    {
        _contractRepository = contractRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(TerminateContractCommand request, CancellationToken cancellationToken)
    {
        var contract = await _contractRepository.GetByIdAsync(request.ContractId, cancellationToken);

        if (contract is null)
        {
            return Result.Failure(new Error("Contract.NotFound", $"Müqavilə tapılmadı: {request.ContractId}"));
        }

        if (request.TerminatedOn.Date < contract.StartDate.Date)
        {
            return Result.Failure(new Error(
                "Contract.InvalidTerminationDate",
                "Ləğv tarixi müqavilənin başlanğıcından əvvəl ola bilməz"));
        }

        contract.Terminate(request.TerminatedOn, request.Note);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

internal sealed class DeleteContractCommandHandler : ICommandHandler<DeleteContractCommand>
{
    private readonly IContractRepository _contractRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteContractCommandHandler(
        IContractRepository contractRepository,
        IUnitOfWork unitOfWork)
    {
        _contractRepository = contractRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteContractCommand request, CancellationToken cancellationToken)
    {
        var contract = await _contractRepository.GetByIdAsync(request.ContractId, cancellationToken);

        if (contract is null)
        {
            return Result.Failure(new Error("Contract.NotFound", $"Müqavilə tapılmadı: {request.ContractId}"));
        }

        if (contract.DeletedAt is not null)
        {
            return Result.Failure(new Error("Contract.AlreadyDeleted", "Müqavilə artıq silinib"));
        }

        if (contract.Status == ContractStatus.Active)
        {
            return Result.Failure(new Error(
                "Contract.Active",
                "Aktiv müqavilə silinə bilməz. Əvvəlcə ləğv edin"));
        }

        contract.Delete();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
