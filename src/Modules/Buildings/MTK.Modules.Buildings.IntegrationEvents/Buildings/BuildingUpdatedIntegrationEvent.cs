using MTK.Common.Application.EventBus;

namespace MTK.Modules.Buildings.IntegrationEvents.Buildings;

/// <summary>
/// Binanın adı/ünvanı dəyişdikdə — Payments öz Building shadow-unu bununla təzələyir.
/// </summary>
public sealed class BuildingUpdatedIntegrationEvent : IntegrationEvent
{
    public BuildingUpdatedIntegrationEvent(
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
