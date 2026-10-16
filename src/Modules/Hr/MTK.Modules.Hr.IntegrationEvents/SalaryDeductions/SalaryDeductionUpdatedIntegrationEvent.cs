using MTK.Common.Application.EventBus;

namespace MTK.Modules.Hr.IntegrationEvents.SalaryDeductions;

public sealed class SalaryDeductionUpdatedIntegrationEvent : IntegrationEvent
{
    public SalaryDeductionUpdatedIntegrationEvent(
        Guid integrationEventId,
        DateTime occurredOnUtc,
        Guid salaryDeductionId,
        Guid employeeId,
        decimal remainingDebt)
        : base(integrationEventId, occurredOnUtc)
    {
        SalaryDeductionId = salaryDeductionId;
        EmployeeId = employeeId;
        RemainingDebt = remainingDebt;
    }

    public Guid SalaryDeductionId { get; }
    public Guid EmployeeId { get; }
    public decimal RemainingDebt { get; }
}
