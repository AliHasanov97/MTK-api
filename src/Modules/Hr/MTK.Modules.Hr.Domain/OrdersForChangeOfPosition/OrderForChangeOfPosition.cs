using MTK.Modules.Hr.Domain.ApplicationsForChangeOfPosition;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Orders;
using MTK.Modules.Hr.Domain.OrdersForChangeOfPosition.Events;

namespace MTK.Modules.Hr.Domain.OrdersForChangeOfPosition;

public sealed class OrderForChangeOfPosition : Order
{
    public Guid ApplicationForChangeOfPositionId { get; private set; }
    public ApplicationForChangeOfPosition ApplicationForChangeOfPosition { get; private set; } = null!;
    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = null!;

    // Override base property to return Employee
    public override Employee? RelatedEmployee => Employee;

    private OrderForChangeOfPosition() { }

    public static OrderForChangeOfPosition Create(
        Guid applicationForChangeOfPositionId,
        Guid employeeId,
        Guid newJobId,
        Guid createdById)
    {
        var order = new OrderForChangeOfPosition
        {
            Id = Guid.NewGuid(),
            Type = OrderType.ChangeOfPosition,
            CreatedById = createdById,
            ApplicationForChangeOfPositionId = applicationForChangeOfPositionId,
            EmployeeId = employeeId
        };

        order.RaiseDomainEvent(new OrderForChangeOfPositionCreatedDomainEvent
        {
            OrderId = order.Id,
            ApplicationForChangeOfPositionId = applicationForChangeOfPositionId,
            EmployeeId = employeeId,
            NewJobId = newJobId,
            CreatedById = createdById
        });

        return order;
    }
}
