using MTK.Common.Application.EventBus;

namespace MTK.Modules.Payments.IntegrationEvents.Payroll;

public sealed class PayrollPaymentCompletedIntegrationEvent : IntegrationEvent
{
    public PayrollPaymentCompletedIntegrationEvent(
        Guid integrationEventId,
        DateTime occurredOnUtc,
        Guid payslipId,
        Guid employeeId,
        decimal courtDeductionAmount,
        int salaryMonth,
        int salaryYear)
        : base(integrationEventId, occurredOnUtc)
    {
        PayslipId = payslipId;
        EmployeeId = employeeId;
        CourtDeductionAmount = courtDeductionAmount;
        SalaryMonth = salaryMonth;
        SalaryYear = salaryYear;
    }

    public Guid PayslipId { get; init; }
    public Guid EmployeeId { get; init; }
    public decimal CourtDeductionAmount { get; init; }
    public int SalaryMonth { get; init; }
    public int SalaryYear { get; init; }
}
