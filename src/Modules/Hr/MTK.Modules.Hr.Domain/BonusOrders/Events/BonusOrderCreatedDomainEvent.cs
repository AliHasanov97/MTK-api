using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.BonusOrders.Events;

public sealed record BonusOrderCreatedDomainEvent : DomainEvent
{
    public Guid BonusOrderId { get; init; }
    public int Index { get; init; }
    public Guid EmployeeId { get; init; }
    public Guid CreatedById { get; init; }

    /// <summary>Manatla mükafat məbləği (bax: BonusOrder.BonusQuantity).</summary>
    public decimal BonusAmount { get; init; }
    public int SalaryMonth { get; init; }
    public int SalaryYear { get; init; }
}
