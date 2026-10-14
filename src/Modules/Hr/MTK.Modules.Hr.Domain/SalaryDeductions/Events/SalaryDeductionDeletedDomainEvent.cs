using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.SalaryDeductions.Events;

public sealed record SalaryDeductionDeletedDomainEvent : DomainEvent
{
    public Guid SalaryDeductionId { get; init; }
    public Guid EmployeeId { get; init; }
}
