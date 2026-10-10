using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.VacationCompensationApplications.Events;

public sealed record VacationCompensationApplicationCreatedDomainEvent : DomainEvent
{
    public Guid ApplicationId { get; init; }
    public Guid EmployeeId { get; init; }
    public int RequestedDays { get; init; }
}