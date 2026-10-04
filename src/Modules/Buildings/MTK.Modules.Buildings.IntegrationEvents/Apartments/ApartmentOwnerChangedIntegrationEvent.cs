using MTK.Common.Application.EventBus;

namespace MTK.Modules.Buildings.IntegrationEvents.Apartments;

/// <summary>
/// Mənzilin sahibi dəyişdikdə digər module-lara bildiriş göndərmək üçün
/// Billing module bu event-i consume edib invoice-ların owner-ini yeniləyir
/// </summary>
public sealed class ApartmentOwnerChangedIntegrationEvent : IntegrationEvent
{
    public ApartmentOwnerChangedIntegrationEvent(
        Guid integrationEventId,
        DateTime occurredOnUtc,
        Guid apartmentId,
        Guid newOwnerId,
        Guid? previousOwnerId,
        decimal areaSquareMeters,
        string apartmentNumber,
        string buildingName,
        string buildingAddress)
        : base(integrationEventId, occurredOnUtc)
    {
        ApartmentId = apartmentId;
        NewOwnerId = newOwnerId;
        PreviousOwnerId = previousOwnerId;
        AreaSquareMeters = areaSquareMeters;
        ApartmentNumber = apartmentNumber;
        BuildingName = buildingName;
        BuildingAddress = buildingAddress;
    }

    public Guid ApartmentId { get; }
    public Guid NewOwnerId { get; }
    public Guid? PreviousOwnerId { get; }
    public decimal AreaSquareMeters { get; }
    public string ApartmentNumber { get; }
    public string BuildingName { get; }
    public string BuildingAddress { get; }
}
