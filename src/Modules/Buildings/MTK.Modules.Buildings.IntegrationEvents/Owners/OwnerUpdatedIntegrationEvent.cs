using MTK.Common.Application.EventBus;

namespace MTK.Modules.Buildings.IntegrationEvents.Owners;

/// <summary>
/// Sahibin əlaqə məlumatı (adı daxil) dəyişdikdə digər modullara bildiriş —
/// Payments bunu öz Owner adi-snapshot-undakı FullName-i yeniləmək üçün consume edir.
/// </summary>
public sealed class OwnerUpdatedIntegrationEvent : IntegrationEvent
{
    public OwnerUpdatedIntegrationEvent(
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
