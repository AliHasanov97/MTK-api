using MTK.Common.Application.Auditing;

namespace MTK.DataSeeder.Infrastructure;

/// <summary>
/// Fake AuditActorAccessor for CLI/Seeder - audit fields üçün lazımdır
/// </summary>
internal sealed class FakeAuditActorAccessor : IAuditActorAccessor
{
    private Guid? _actorUserId = Guid.Parse("00000000-0000-0000-0000-000000000001"); // System user

    public Guid? ActorUserId => _actorUserId;

    public void SetActorUserId(Guid? userId)
    {
        _actorUserId = userId;
    }
}
