using MTK.Common.Application.Auditing;

namespace MTK.Common.Infrastructure.Auditing;

internal sealed class AuditActorAccessor : IAuditActorAccessor
{
    public Guid? ActorUserId { get; private set; }

    public void SetActorUserId(Guid? userId) => ActorUserId = userId;
}
