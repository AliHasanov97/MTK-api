using MTK.Common.Application.EventBus;

namespace MTK.Modules.Buildings.IntegrationEvents.Owners;

/// <summary>
/// Yeni sahib (aktiv və ya passiv) yaradıldıqda digər modullara bildiriş —
/// Payments bunu öz Owner adi-snapshot-unu (FullName, qəbz/sənəd üçün) qurmaq üçün
/// consume edir.
/// </summary>
public sealed class OwnerCreatedIntegrationEvent : IntegrationEvent
{
    public OwnerCreatedIntegrationEvent(
        Guid integrationEventId,
        DateTime occurredOnUtc,
        Guid ownerId,
        string fullName)
        : base(integrationEventId, occurredOnUtc)
    {
        OwnerId = ownerId;
        FullName = fullName;
    }

    public Guid OwnerId { get; }
    public string FullName { get; }
}
