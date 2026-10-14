using MTK.Common.Application.EventBus;

namespace MTK.Modules.Hr.IntegrationEvents.Employees;

/// <summary>
/// Yeni işçi yaradıldıqda digər modullara bildiriş — Payments bunu öz Employee
/// adi-snapshot-unu (əmək haqqı hesablanması üçün) qurmaq üçün consume edir.
/// </summary>
public sealed class EmployeeCreatedIntegrationEvent : IntegrationEvent
{
    public EmployeeCreatedIntegrationEvent(
        Guid integrationEventId,
        DateTime occurredOnUtc,
        Guid employeeId,
        string fullName,
        decimal baseSalary)
        : base(integrationEventId, occurredOnUtc)
    {
        EmployeeId = employeeId;
        FullName = fullName;
        BaseSalary = baseSalary;
    }

    public Guid EmployeeId { get; }
    public string FullName { get; }
    public decimal BaseSalary { get; }
}
