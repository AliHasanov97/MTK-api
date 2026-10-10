using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.EmployeeWorkHistories;

namespace MTK.Modules.Hr.Application.EmployeeWorkHistories.DeleteEmployeeWorkHistory;

internal sealed class DeleteEmployeeWorkHistoryCommandHandler : ICommandHandler<DeleteEmployeeWorkHistoryCommand>
{
    private readonly IEmployeeWorkHistoryRepository _workHistoryRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEmployeeWorkHistoryCommandHandler(
        IEmployeeWorkHistoryRepository workHistoryRepository,
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork)
    {
        _workHistoryRepository = workHistoryRepository;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteEmployeeWorkHistoryCommand request, CancellationToken cancellationToken)
    {
        var workHistory = await _workHistoryRepository.GetByIdDefaultAsync(request.Id, cancellationToken);
        if (workHistory is null)
            return Result.Failure(EmployeeWorkHistoryErrors.NotFound);

        // Silinmə domain event-i at
        workHistory.Delete();

        await _workHistoryRepository.DeleteAsync(workHistory, null, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
