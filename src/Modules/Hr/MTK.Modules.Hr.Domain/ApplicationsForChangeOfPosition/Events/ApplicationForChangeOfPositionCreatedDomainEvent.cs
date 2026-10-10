using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.ApplicationsForChangeOfPosition.Events;

public sealed record ApplicationForChangeOfPositionCreatedDomainEvent : DomainEvent
{
    public Guid ApplicationId { get; init; }
    public int ApplicationNumber { get; init; }
    public DateTimeOffset ApplicationDate { get; init; }
}
