using MTK.Common.Application.EventBus;

namespace MTK.Modules.Buildings.IntegrationEvents.Buildings;

/// <summary>
/// Yeni bina yaradıldıqda digər module-lara bildiriş göndərmək üçün — Payments bu
/// event-i consume edib öz Building shadow-unu yaradır. Bundan əvvəl bina yalnız
/// ilk mənzili yaradılanda (ApartmentCreated vasitəsilə) dolayı yolla sinxronlaşırdı;
/// mənzilsiz/hələ boş bina Payments-də heç görünmürdü.
/// </summary>
public sealed class BuildingCreatedIntegrationEvent : IntegrationEvent
{
    public BuildingCreatedIntegrationEvent(
        Guid integrationEventId,
        DateTime occurredOnUtc,
        Guid buildingId,
        string name,
        string address)
        : base(integrationEventId, occurredOnUtc)
    {
        BuildingId = buildingId;
        Name = name;
        Address = address;
    }

    public Guid BuildingId { get; }
    public string Name { get; }
    public string Address { get; }
}
