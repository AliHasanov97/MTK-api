using MTK.Common.Application.EventBus;

namespace MTK.Modules.Buildings.IntegrationEvents.Garages;

/// <summary>
/// Qarajın sahibi dəyişdikdə digər module-lara bildiriş göndərmək üçün
/// Billing module bu event-i consume edib invoice-ların owner-ini yeniləyir
/// </summary>
public sealed class GarageOwnerChangedIntegrationEvent : IntegrationEvent
{
    public GarageOwnerChangedIntegrationEvent(
        Guid integrationEventId,
        DateTime occurredOnUtc,
        Guid garageId,
        Guid newOwnerId,
        Guid? previousOwnerId,
        string garageNumber,
        string garageType)
        : base(integrationEventId, occurredOnUtc)
    {
        GarageId = garageId;
        NewOwnerId = newOwnerId;
        PreviousOwnerId = previousOwnerId;
        GarageNumber = garageNumber;
        GarageType = garageType;
    }

    public Guid GarageId { get; }
    public Guid NewOwnerId { get; }
    public Guid? PreviousOwnerId { get; }
    public string GarageNumber { get; }
    public string GarageType { get; }
}
