using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.UnexcusedAbsences.Events;

public sealed record UnexcusedAbsenceCreatedDomainEvent : DomainEvent
{
    public Guid UnexcusedAbsenceId { get; init; }
    public int Index { get; init; }
    public DateTimeOffset SetDate { get; init; }
    public Guid EmployeeId { get; init; }
    public Guid CreatedById { get; init; }
}