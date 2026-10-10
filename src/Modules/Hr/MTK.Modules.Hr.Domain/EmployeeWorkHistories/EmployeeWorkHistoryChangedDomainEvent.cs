using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.EmployeeWorkHistories;

public sealed record EmployeeWorkHistoryChangedDomainEvent : DomainEvent
{
    public Guid EmployeeId { get; init; }
}
