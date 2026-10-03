namespace MTK.Common.Infrastructure.Outbox;

public sealed class OutboxMessage
{
    public Guid Id { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public DateTimeOffset OccurredOnUtc { get; init; }
    public DateTimeOffset? ProcessedOnUtc { get; init; }
    public string? Error { get; init; }

    // Who triggered the original SaveChanges that raised this domain event (resolved
    // from HttpContext at capture time) — replayed into IAuditActorAccessor before the
    // event's handler runs in the outbox job's own scope, so audit logs for entities
    // created/updated there (e.g. the Transaction ledger entry from a completed
    // payment) still point at the real actor instead of null.
    public Guid? TriggeredByUserId { get; init; }
}
