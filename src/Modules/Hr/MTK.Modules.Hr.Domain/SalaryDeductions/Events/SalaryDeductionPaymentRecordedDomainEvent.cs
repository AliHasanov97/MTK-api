using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.SalaryDeductions.Events;

public sealed record SalaryDeductionPaymentRecordedDomainEvent : DomainEvent
{
    public Guid SalaryDeductionId { get; init; }
    public Guid EmployeeId { get; init; }
    public decimal AmountPaid { get; init; }
    public decimal RemainingDebt { get; init; }
}
