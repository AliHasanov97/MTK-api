using MTK.Common.Application.EventBus;

namespace MTK.Modules.Hr.IntegrationEvents.SalaryDeductions;

public sealed class SalaryDeductionUpdatedIntegrationEvent : IntegrationEvent
{
    public SalaryDeductionUpdatedIntegrationEvent(
        Guid integrationEventId,
        DateTime occurredOnUtc,
        Guid salaryDeductionId,
        Guid employeeId,
        decimal stateFeeRemaining,
        decimal debtRemaining)
        : base(integrationEventId, occurredOnUtc)
    {
        SalaryDeductionId = salaryDeductionId;
        EmployeeId = employeeId;
        StateFeeRemaining = stateFeeRemaining;
        DebtRemaining = debtRemaining;
    }

    public Guid SalaryDeductionId { get; }
    public Guid EmployeeId { get; }
    public decimal StateFeeRemaining { get; }
    public decimal DebtRemaining { get; }
}
