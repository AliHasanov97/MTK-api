using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.Employees.Events;

public sealed record EmployeeUpdatedDomainEvent : DomainEvent
{
    public Guid EmployeeId { get; init; }
}
