using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Warehouse.Domain.Nomenclatures;
using MTK.Modules.Warehouse.Domain.Nomenclatures.Events;
using MTK.Modules.Warehouse.IntegrationEvents.Nomenclatures;

namespace MTK.Modules.Warehouse.Application.Nomenclatures.Events;

internal sealed class NomenclatureCreatedDomainEventHandler : DomainEventHandler<NomenclatureCreatedDomainEvent>
{
    private readonly INomenclatureRepository _nomenclatureRepository;
    private readonly IEventBus _eventBus;

    public NomenclatureCreatedDomainEventHandler(
        INomenclatureRepository nomenclatureRepository,
        IEventBus eventBus)
    {
        _nomenclatureRepository = nomenclatureRepository;
        _eventBus = eventBus;
    }

    public override async Task Handle(
        NomenclatureCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        // Domain event yalnız Id daşıyır — tam detalları aqreqatdan oxuyuruq.
        var nomenclature = await _nomenclatureRepository.GetByIdAsync(
            domainEvent.NomenclatureId,
            cancellationToken);

        if (nomenclature is null)
        {
            return;
        }

        var integrationEvent = new NomenclatureCreatedIntegrationEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            nomenclature.Id,
            nomenclature.Code,
            nomenclature.Name,
            nomenclature.Description,
            (int)nomenclature.Category,
            (int)nomenclature.Unit,
            nomenclature.MinStockLevel,
            nomenclature.IsActive);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
