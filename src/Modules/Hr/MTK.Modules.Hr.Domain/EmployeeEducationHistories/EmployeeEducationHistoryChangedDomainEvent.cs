using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.EmployeeEducationHistories;

public sealed record EmployeeEducationHistoryChangedDomainEvent : DomainEvent
{
    public Guid EmployeeId { get; init; }
}
