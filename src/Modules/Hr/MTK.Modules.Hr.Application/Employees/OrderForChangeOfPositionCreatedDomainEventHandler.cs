using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.OrdersForChangeOfPosition.Events;

namespace MTK.Modules.Hr.Application.Employees;

internal sealed class OrderForChangeOfPositionCreatedDomainEventHandler
    : DomainEventHandler<OrderForChangeOfPositionCreatedDomainEvent>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OrderForChangeOfPositionCreatedDomainEventHandler(
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public override async Task Handle(
        OrderForChangeOfPositionCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdDefaultAsync(domainEvent.EmployeeId, cancellationToken);
        if (employee is null) return;

        employee.UpdatePosition(domainEvent.NewJobId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
