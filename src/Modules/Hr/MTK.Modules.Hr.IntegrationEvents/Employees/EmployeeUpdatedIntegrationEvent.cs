using MTK.Common.Application.EventBus;

namespace MTK.Modules.Hr.IntegrationEvents.Employees;

/// <summary>
/// İşçinin adı və ya əmək haqqı dəyişdikdə digər modullara bildiriş — Payments
/// bunu öz Employee snapshot-unu (FullName, BaseSalary) yeniləmək üçün consume edir.
/// </summary>
public sealed class EmployeeUpdatedIntegrationEvent : IntegrationEvent
{
    public EmployeeUpdatedIntegrationEvent(
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
