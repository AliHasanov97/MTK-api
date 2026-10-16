using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Domain.Payroll.Events;

public sealed record PayslipPaidDomainEvent : DomainEvent
{
    public Guid PayslipId { get; init; }
    public Guid EmployeeId { get; init; }
    public decimal CourtDeductionAmount { get; init; }
    public int SalaryMonth { get; init; }
    public int SalaryYear { get; init; }
    public PaymentMethod PaymentMethod { get; init; }
    public DateTimeOffset PaidAt { get; init; }
}
