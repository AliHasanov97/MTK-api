using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Employees;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Employees.Commands.SyncEmployee;

internal sealed class SyncEmployeeCommandHandler : ICommandHandler<SyncEmployeeCommand>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SyncEmployeeCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SyncEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdDefaultAsync(request.EmployeeId, cancellationToken);
        if (employee is not null)
        {
            employee.Update(request.FullName, request.BaseSalary, request.IsActive);
        }
        else
        {
            employee = Employee.Create(request.EmployeeId, request.FullName, request.BaseSalary, request.IsActive);
            _employeeRepository.Add(employee);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
