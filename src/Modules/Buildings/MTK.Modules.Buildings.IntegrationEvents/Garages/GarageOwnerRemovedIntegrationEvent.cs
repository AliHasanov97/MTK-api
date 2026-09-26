using MTK.Common.Application.EventBus;

namespace MTK.Modules.Buildings.IntegrationEvents.Garages;

/// <summary>
/// Qarajdan sahib çıxarıldıqda digər module-lara bildiriş göndərmək üçün
/// </summary>
public sealed class GarageOwnerRemovedIntegrationEvent : IntegrationEvent
{
    public GarageOwnerRemovedIntegrationEvent(
        Guid integrationEventId,
        DateTime occurredOnUtc,
        Guid garageId,
        Guid removedOwnerId)
        : base(integrationEventId, occurredOnUtc)
    {
        GarageId = garageId;
        RemovedOwnerId = removedOwnerId;
    }

    public Guid GarageId { get; }
    public Guid RemovedOwnerId { get; }
}
