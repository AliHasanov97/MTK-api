using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.EmploymentStatusChangeApplications;
using MTK.Modules.Hr.Domain.EmploymentStatusChangeOrders.Events;
using MTK.Modules.Hr.Domain.Orders;

namespace MTK.Modules.Hr.Domain.EmploymentStatusChangeOrders;

public sealed class EmploymentStatusChangeOrder : Order
{
    public Guid EmploymentStatusChangeApplicationId { get; private set; }
    public EmploymentStatusChangeApplication EmploymentStatusChangeApplication { get; private set; } = null!;

    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = null!;

    public override Employee? RelatedEmployee => Employee;

    public EmploymentType CurrentEmploymentType { get; private set; }
    public EmploymentType NewEmploymentType { get; private set; }

    public Guid OrderExecutionSupervisorId { get; private set; }
    public Employee OrderExecutionSupervisor { get; private set; } = null!;
    
    private EmploymentStatusChangeOrder() { }

    public static EmploymentStatusChangeOrder Create(
        Guid employmentStatusChangeApplicationId,
        Guid employeeId,
        EmploymentType currentEmploymentType,
        EmploymentType newEmploymentType,
        Guid orderExecutionSupervisorId,
        Guid createdById)
    {
        var order = new EmploymentStatusChangeOrder
        {
            Id = Guid.NewGuid(),
            Type = OrderType.EmploymentStatusChange,
            CreatedById = createdById,
            EmploymentStatusChangeApplicationId = employmentStatusChangeApplicationId,
            EmployeeId = employeeId,
            CurrentEmploymentType = currentEmploymentType,
            NewEmploymentType = newEmploymentType,
            OrderExecutionSupervisorId = orderExecutionSupervisorId
        };

        order.RaiseDomainEvent(new EmploymentStatusChangeOrderCreatedDomainEvent
        {
            OrderId = order.Id,
            EmploymentStatusChangeApplicationId = employmentStatusChangeApplicationId,
            EmployeeId = employeeId,
            NewEmploymentType = newEmploymentType,
            OrderExecutionSupervisorId = orderExecutionSupervisorId,
            CreatedById = createdById
        });

        return order;
    }
}