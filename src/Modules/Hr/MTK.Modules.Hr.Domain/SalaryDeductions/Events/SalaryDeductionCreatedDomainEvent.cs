using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.SalaryDeductions.Events;

public sealed record SalaryDeductionCreatedDomainEvent : DomainEvent
{
    public Guid SalaryDeductionId { get; init; }
    public Guid EmployeeId { get; init; }
    public decimal PercentageSalary { get; init; }
    public DateTimeOffset StartDate { get; init; }
}
