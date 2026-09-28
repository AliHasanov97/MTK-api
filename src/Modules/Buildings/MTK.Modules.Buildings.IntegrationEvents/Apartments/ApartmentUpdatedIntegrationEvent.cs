using MTK.Common.Application.EventBus;

namespace MTK.Modules.Buildings.IntegrationEvents.Apartments;

/// <summary>
/// Mənzil məlumatları (sahə, otaq sayı) dəyişdikdə digər module-lara bildiriş göndərmək üçün
/// Billing module bu event-i consume edib tariflər üçün sahə məlumatını yeniləyir
/// </summary>
public sealed class ApartmentUpdatedIntegrationEvent : IntegrationEvent
{
    public ApartmentUpdatedIntegrationEvent(
        Guid integrationEventId,
        DateTime occurredOnUtc,
        Guid apartmentId,
        decimal areaSquareMeters)
        : base(integrationEventId, occurredOnUtc)
    {
        ApartmentId = apartmentId;
        AreaSquareMeters = areaSquareMeters;
    }

    public Guid ApartmentId { get; }
    public decimal AreaSquareMeters { get; }
}
