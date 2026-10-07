using MTK.Common.Application.EventBus;

namespace MTK.Modules.Warehouse.IntegrationEvents.Nomenclatures;

/// <summary>
/// Nomenklatura silindikdə (soft delete) digər modullara bildiriş. Event yalnız
/// Id daşıyır — silinmiş aqreqatın detalları oxuna bilməz.
/// </summary>
public sealed class NomenclatureDeletedIntegrationEvent : IntegrationEvent
{
    public NomenclatureDeletedIntegrationEvent(
        Guid integrationEventId,
        DateTime occurredOnUtc,
        Guid nomenclatureId)
        : base(integrationEventId, occurredOnUtc)
    {
        NomenclatureId = nomenclatureId;
    }

    public Guid NomenclatureId { get; }
}
