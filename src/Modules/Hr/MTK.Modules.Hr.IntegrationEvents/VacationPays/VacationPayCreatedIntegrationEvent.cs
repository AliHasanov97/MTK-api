using MTK.Common.Application.EventBus;

namespace MTK.Modules.Hr.IntegrationEvents.VacationPays;

/// <summary>
/// Məzuniyyət haqqı yaradılması tələbi — VacationOrder yaradıldıqda
/// Payments moduluna göndərilir ki, VacationPay entity-si yaradılsın.
/// Orta əmək haqqı hesablanması Payments modulunda aparılır (circular dependency-dən qaçmaq üçün).
/// </summary>
public sealed class VacationPayCreatedIntegrationEvent : IntegrationEvent
{
    public VacationPayCreatedIntegrationEvent(
        Guid integrationEventId,
        DateTime occurredOnUtc,
        Guid vacationOrderId,
        Guid employeeId,
        string employeeFullName,
        int vacationDays,
        DateTimeOffset vacationStartDate,
        DateTimeOffset vacationEndDate)
        : base(integrationEventId, occurredOnUtc)
    {
        VacationOrderId = vacationOrderId;
        EmployeeId = employeeId;
        EmployeeFullName = employeeFullName;
        VacationDays = vacationDays;
        VacationStartDate = vacationStartDate;
        VacationEndDate = vacationEndDate;
    }

    public Guid VacationOrderId { get; }
    public Guid EmployeeId { get; }
    public string EmployeeFullName { get; }
    public int VacationDays { get; }
    public DateTimeOffset VacationStartDate { get; }
    public DateTimeOffset VacationEndDate { get; }
}
