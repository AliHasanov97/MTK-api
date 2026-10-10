using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.Employees.Events;

public sealed record EmployeeCreatedDomainEvent : DomainEvent
{
    public Guid EmployeeId { get; init; }
    public DateTimeOffset StartWorkDate { get; init; }
}
