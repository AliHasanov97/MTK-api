using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.OrdersForChangeOfPosition.Events;

public sealed record OrderForChangeOfPositionCreatedDomainEvent : DomainEvent
{
    public Guid OrderId { get; init; }
    public int OrderNumber { get; init; }
    public Guid ApplicationForChangeOfPositionId { get; init; }
    public Guid EmployeeId { get; init; }
    public Guid NewJobId { get; init; }
    public Guid CreatedById { get; init; }
}
