using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.BonusOrders.Events;

public sealed record BonusOrderCreatedDomainEvent : DomainEvent
{
    public Guid BonusOrderId { get; init; }
    public int Index { get; init; }
    public Guid EmployeeId { get; init; }
    public Guid CreatedById { get; init; }
}
