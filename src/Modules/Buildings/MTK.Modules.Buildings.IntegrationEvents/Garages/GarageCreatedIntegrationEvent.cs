using MTK.Common.Application.EventBus;

namespace MTK.Modules.Buildings.IntegrationEvents.Garages;

/// <summary>
/// Yeni qaraj yaradıldıqda digər module-lara bildiriş göndərmək üçün
/// Billing module bu event-i consume edib premium qaraj üçün əlavə tərifə tətbiq edir
/// </summary>
public sealed class GarageCreatedIntegrationEvent : IntegrationEvent
{
    public GarageCreatedIntegrationEvent(
        Guid integrationEventId,
        DateTime occurredOnUtc,
        Guid garageId,
        Guid? ownerId,
        string garageNumber,
        string garageType)
        : base(integrationEventId, occurredOnUtc)
    {
        GarageId = garageId;
        OwnerId = ownerId;
        GarageNumber = garageNumber;
        GarageType = garageType;
    }

    public Guid GarageId { get; }
    public Guid? OwnerId { get; }
    public string GarageNumber { get; }
    public string GarageType { get; }
}
