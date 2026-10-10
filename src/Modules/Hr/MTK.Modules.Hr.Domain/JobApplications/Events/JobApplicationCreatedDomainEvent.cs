using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.JobApplications.Events;

public sealed record JobApplicationCreatedDomainEvent : DomainEvent
{
    public Guid JobApplicationId { get; init; }
    public int ApplicationNumber { get; init; }
    public DateTimeOffset ApplicationDate { get; init; }
}
