using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Helpers;
using MTK.Modules.Hr.Domain.Orders;
using MTK.Modules.Hr.Domain.Warnings.Events;

namespace MTK.Modules.Hr.Domain.Warnings;

public sealed class WarningOrder : Order
{
    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = null!;

    // Override base property to return Employee
    public override Employee? RelatedEmployee => Employee;

    public Guid OrderExecutionSupervisorId { get; private set; }
    public Employee OrderExecutionSupervisor { get; private set; } = null!;

    public DateTimeOffset SetDate { get; private set; }

    public DisciplinaryType DisciplinaryType { get; private set; }

    private WarningOrder() { }

    public static WarningOrder Create(
        DisciplinaryType disciplinaryType,
        Guid employeeId,
        Guid orderExecutionSupervisorId,
        DateTimeOffset setDate,
        Guid createdById)
    {
        var warning = new WarningOrder
        {
            Id = Guid.NewGuid(),
            Type = MapToOrderType(disciplinaryType),
            DisciplinaryType = disciplinaryType,
            CreatedById = createdById,
            EmployeeId = employeeId,
            OrderExecutionSupervisorId = orderExecutionSupervisorId,
            SetDate = DateTimeHelper.ToUtcDateOnly(setDate)
        };

        warning.RaiseDomainEvent(new WarningCreatedDomainEvent
        {
            WarningId = warning.Id,
            SetDate = DateTimeHelper.ToUtcDateOnly(setDate),
            EmployeeId = employeeId,
            CreatedById = createdById
        });

        return warning;
    }
    

    private static OrderType MapToOrderType(DisciplinaryType disciplinaryType)
    {
        return disciplinaryType switch
        {
            DisciplinaryType.Warning => OrderType.Warning,
            DisciplinaryType.Reprimand => OrderType.Reprimand,
            DisciplinaryType.SevereReprimand => OrderType.SevereReprimand,
            _ => OrderType.Warning
        };
    }
}