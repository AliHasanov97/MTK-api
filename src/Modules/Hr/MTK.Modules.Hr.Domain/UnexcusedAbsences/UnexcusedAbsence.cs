using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Helpers;
using MTK.Modules.Hr.Domain.Orders;
using MTK.Modules.Hr.Domain.UnexcusedAbsences.Events;

namespace MTK.Modules.Hr.Domain.UnexcusedAbsences;

public sealed class UnexcusedAbsenceOrder : Order
{
    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = null!;

    // Override base property to return Employee
    public override Employee? RelatedEmployee => Employee;

    public DateTimeOffset SetDate { get; private set; }

    private UnexcusedAbsenceOrder() { }

    public static UnexcusedAbsenceOrder Create(
        Guid employeeId,
        DateTimeOffset setDate,
        Guid createdById)
    {
        var absence = new UnexcusedAbsenceOrder
        {
            Id = Guid.NewGuid(),
            Type = OrderType.UnexcusedAbsence,
            CreatedById = createdById,
            EmployeeId = employeeId,
            SetDate = DateTimeHelper.ToUtcDateOnly(setDate)
        };

        absence.RaiseDomainEvent(new UnexcusedAbsenceCreatedDomainEvent
        {
            UnexcusedAbsenceId = absence.Id,
            SetDate = DateTimeHelper.ToUtcDateOnly(setDate),
            EmployeeId = employeeId,
            CreatedById = createdById
        });

        return absence;
    }

}
