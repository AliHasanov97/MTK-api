namespace MTK.Common.Infrastructure.Outbox;

/// <summary>
/// Implemented by each module's DbContext so InsertOutboxMessagesInterceptor can read
/// the actor it already resolved for this SaveChanges call (HttpContext, or the
/// IAuditActorAccessor override set by ProcessOutboxJobBase) without depending on any
/// specific module's DbContext type.
/// </summary>
public interface IHasAuditActor
{
    Guid? CurrentActorUserId { get; }
}
