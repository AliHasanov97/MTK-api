using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.UnpaidLeaveOrders.Events;

public sealed record UnpaidLeaveOrderCreatedDomainEvent : DomainEvent
{
    public Guid OrderId { get; init; }
    public Guid EmployeeId { get; init; }
    public DateTimeOffset StartDate { get; init; }
    public DateTimeOffset EndDate { get; init; }
}