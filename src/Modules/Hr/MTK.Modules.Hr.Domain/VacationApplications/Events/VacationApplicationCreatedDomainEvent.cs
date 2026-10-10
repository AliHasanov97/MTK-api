using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.VacationApplications.Events;

public sealed record VacationApplicationCreatedDomainEvent : DomainEvent
{
    public Guid ApplicationId { get; init; }
    public Guid EmployeeId { get; init; }
    public DateTimeOffset StartDate { get; init; }
    public DateTimeOffset EndDate { get; init; }
    public int RequestedDays { get; init; }
}
