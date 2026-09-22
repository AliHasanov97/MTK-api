using MTK.Common.Application.EventBus;

namespace MTK.Modules.Identity.IntegrationEvents.Users;

/// <summary>
/// Identity modulunda User silindikdə digər module-lara bildiriş göndərmək üçün
/// Buildings module bu event-i consume edib Owner-i deaktiv edir (soft delete)
/// </summary>
public sealed class UserDeletedIntegrationEvent : IntegrationEvent
{
    public UserDeletedIntegrationEvent(
        Guid integrationEventId,
        DateTime occurredOnUtc,
        Guid userId)
        : base(integrationEventId, occurredOnUtc)
    {
        UserId = userId;
    }

    public Guid UserId { get; }
}
