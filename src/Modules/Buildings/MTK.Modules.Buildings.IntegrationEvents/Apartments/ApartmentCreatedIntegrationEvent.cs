using MTK.Common.Application.EventBus;

namespace MTK.Modules.Buildings.IntegrationEvents.Apartments;

/// <summary>
/// Yeni mənzil yaradıldıqda digər module-lara bildiriş göndərmək üçün
/// Billing module bu event-i consume edib ServiceRate və ya digər məlumatları hazırlaya bilər
/// </summary>
public sealed class ApartmentCreatedIntegrationEvent : IntegrationEvent
{
    public ApartmentCreatedIntegrationEvent(
        Guid integrationEventId,
        DateTime occurredOnUtc,
        Guid apartmentId,
        Guid buildingId,
        string apartmentNumber,
        decimal areaSquareMeters)
        : base(integrationEventId, occurredOnUtc)
    {
        ApartmentId = apartmentId;
        BuildingId = buildingId;
        ApartmentNumber = apartmentNumber;
        AreaSquareMeters = areaSquareMeters;
    }

    public Guid ApartmentId { get; }
    public Guid BuildingId { get; }
    public string ApartmentNumber { get; }
    public decimal AreaSquareMeters { get; }
}
