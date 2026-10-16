using MTK.Common.Application.EventBus;

namespace MTK.Modules.Hr.IntegrationEvents.SalaryDeductions;

/// <summary>
/// Məhkəmə qərarı ilə əmək haqqından tutma əmri yaradıldıqda digər modullara
/// bildiriş — Payments bunu hər ay net əmək haqqından tutmaq üçün consume edir.
/// </summary>
public sealed class SalaryDeductionCreatedIntegrationEvent : IntegrationEvent
{
    public SalaryDeductionCreatedIntegrationEvent(
        Guid integrationEventId,
        DateTime occurredOnUtc,
        Guid salaryDeductionId,
        Guid employeeId,
        decimal percentageSalary,
        DateTimeOffset startDate,
        decimal remainingDebt)
        : base(integrationEventId, occurredOnUtc)
    {
        SalaryDeductionId = salaryDeductionId;
        EmployeeId = employeeId;
        PercentageSalary = percentageSalary;
        StartDate = startDate;
        RemainingDebt = remainingDebt;
    }

    public Guid SalaryDeductionId { get; }
    public Guid EmployeeId { get; }
    public decimal PercentageSalary { get; }
    public DateTimeOffset StartDate { get; }
    public decimal RemainingDebt { get; }
}
