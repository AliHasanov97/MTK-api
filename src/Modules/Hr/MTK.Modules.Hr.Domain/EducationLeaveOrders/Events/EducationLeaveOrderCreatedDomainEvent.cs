using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.EducationLeaveOrders.Events;

public sealed record EducationLeaveOrderCreatedDomainEvent : DomainEvent
{
    public required Guid OrderId { get; init; }
    public required Guid EmployeeId { get; init; }
    public required DateTimeOffset StartDate { get; init; }
    public required DateTimeOffset EndDate { get; init; }
}