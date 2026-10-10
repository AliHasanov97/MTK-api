using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.Employees;

namespace MTK.Modules.Hr.Application.Employees.DeleteEmployee;

internal sealed class DeleteEmployeeCommandHandler
    : ICommandHandler<DeleteEmployeeCommand>
{
    private readonly IEmployeeRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEmployeeCommandHandler(
        IEmployeeRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);
        if (employee is null)
            return Result.Failure(EmployeeErrors.NotFound);
        if (employee.EmploymentOrderId.HasValue)
            return Result.Failure(EmployeeErrors.CannotDeleteHiredViaAcceptanceOrder);

        await _repository.DeleteAsync(employee, null, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}