using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.EmployeeEducationHistories;

namespace MTK.Modules.Hr.Application.EmployeeEducationHistories.DeleteEmployeeEducationHistory;

internal sealed class DeleteEmployeeEducationHistoryCommandHandler
    : ICommandHandler<DeleteEmployeeEducationHistoryCommand>
{
    private readonly IEmployeeEducationHistoryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEmployeeEducationHistoryCommandHandler(
        IEmployeeEducationHistoryRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteEmployeeEducationHistoryCommand request, CancellationToken cancellationToken)
    {
        var history = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);
        if (history is null)
            return Result.Failure(EmployeeEducationHistoryErrors.NotFound);

        await _repository.DeleteAsync(history, null, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
