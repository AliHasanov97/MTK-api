using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.Warnings.Events;

public sealed record WarningCreatedDomainEvent : DomainEvent
{
    public Guid WarningId { get; init; }
    public int Index { get; init; }
    public DateTimeOffset SetDate { get; init; }
    public Guid EmployeeId { get; init; }
    public Guid CreatedById { get; init; }
}