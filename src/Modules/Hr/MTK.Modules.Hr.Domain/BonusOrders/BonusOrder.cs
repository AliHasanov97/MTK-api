using MTK.Modules.Hr.Domain.BonusOrders.Events;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Helpers;
using MTK.Modules.Hr.Domain.Orders;

namespace MTK.Modules.Hr.Domain.BonusOrders;

public sealed class BonusOrder : Order
{
    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = null!;

    public override Employee? RelatedEmployee => Employee;

    public Guid OrderExecutionSupervisorId { get; private set; }
    public Employee OrderExecutionSupervisor { get; private set; } = null!;

    public int BonusQuantity { get; private set; }
    public int SalaryMonth { get; private set; }
    public int SalaryYear { get; private set; }

    private BonusOrder() { }

    public static BonusOrder Create(
        Guid employeeId,
        Guid orderExecutionSupervisorId,
        int bonusQuantity,
        int salaryMonth,
        int salaryYear,
        Guid createdById)
    {
        var bonusOrder = new BonusOrder
        {
            Id = Guid.NewGuid(),
            Type = OrderType.Bonus,
            CreatedById = createdById,
            EmployeeId = employeeId,
            OrderExecutionSupervisorId = orderExecutionSupervisorId,
            BonusQuantity = bonusQuantity,
            SalaryMonth = salaryMonth,
            SalaryYear = salaryYear
        };

        bonusOrder.RaiseDomainEvent(new BonusOrderCreatedDomainEvent
        {
            BonusOrderId = bonusOrder.Id,
            EmployeeId = employeeId,
            CreatedById = createdById
        });

        return bonusOrder;
    }
}
