using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Modules.Warehouse.Domain.Nomenclatures.Events;
using MTK.Modules.Warehouse.IntegrationEvents.Nomenclatures;

namespace MTK.Modules.Warehouse.Application.Nomenclatures.Events;

internal sealed class NomenclatureDeletedDomainEventHandler : DomainEventHandler<NomenclatureDeletedDomainEvent>
{
    private readonly IEventBus _eventBus;

    public NomenclatureDeletedDomainEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public override async Task Handle(
        NomenclatureDeletedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        // Soft delete: aqreqat artıq oxuna bilməz (global query filter), ona görə
        // yalnız Id göndəririk.
        var integrationEvent = new NomenclatureDeletedIntegrationEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            domainEvent.NomenclatureId);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
