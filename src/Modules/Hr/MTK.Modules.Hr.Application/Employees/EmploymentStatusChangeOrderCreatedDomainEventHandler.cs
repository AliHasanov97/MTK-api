using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.EmploymentStatusChangeOrders.Events;

namespace MTK.Modules.Hr.Application.Employees;

internal sealed class EmploymentStatusChangeOrderCreatedDomainEventHandler
    : DomainEventHandler<EmploymentStatusChangeOrderCreatedDomainEvent>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EmploymentStatusChangeOrderCreatedDomainEventHandler(
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public override async Task Handle(
        EmploymentStatusChangeOrderCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdDefaultAsync(domainEvent.EmployeeId, cancellationToken);
        if (employee is null) return;

        employee.UpdateEmploymentType(domainEvent.NewEmploymentType);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
