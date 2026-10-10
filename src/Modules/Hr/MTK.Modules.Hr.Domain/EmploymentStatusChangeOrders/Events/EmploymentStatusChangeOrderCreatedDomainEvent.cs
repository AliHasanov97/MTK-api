using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.Employees;

namespace MTK.Modules.Hr.Domain.EmploymentStatusChangeOrders.Events;

public sealed record EmploymentStatusChangeOrderCreatedDomainEvent : DomainEvent
{
    public Guid OrderId { get; init; }
    public int OrderNumber { get; init; }
    public Guid EmploymentStatusChangeApplicationId { get; init; }
    public Guid EmployeeId { get; init; }
    public EmploymentType NewEmploymentType { get; init; }
    public Guid OrderExecutionSupervisorId { get; init; }
    public Guid CreatedById { get; init; }
}
