namespace MTK.Common.Application.Auditing;

/// <summary>
/// Scoped override for "who performed this" when a SaveChanges call happens outside
/// an HTTP request (e.g. a domain-event handler run by a background outbox job). The
/// request-bound actor is captured into the outbox message when it's first written,
/// then replayed here before the handler runs, so the resulting audit log still points
/// at the real actor instead of falling back to null.
/// </summary>
public interface IAuditActorAccessor
{
    Guid? ActorUserId { get; }

    void SetActorUserId(Guid? userId);
}
