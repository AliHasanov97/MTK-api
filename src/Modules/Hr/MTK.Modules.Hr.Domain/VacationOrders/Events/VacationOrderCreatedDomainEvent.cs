using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.VacationOrders.Events;

public sealed record VacationOrderCreatedDomainEvent : DomainEvent
{
    public Guid OrderId { get; init; }
    public Guid EmployeeId { get; init; }
    public DateTimeOffset StartDate { get; init; }
    public DateTimeOffset EndDate { get; init; }
    public int VacationDays { get; init; }
}
