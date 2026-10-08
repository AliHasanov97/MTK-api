using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Nomenclatures.Commands.SyncNomenclature;
using MTK.Modules.Warehouse.IntegrationEvents.Nomenclatures;

namespace MTK.Modules.Payments.Presentation.Nomenclatures;

/// <summary>
/// Warehouse-da nomenklatura yaradılanda bu moduldakı güzgünü yaradır — alış sətirləri
/// bu güzgüyə istinad edir.
/// </summary>
internal sealed class SyncNomenclatureOnCreatedIntegrationEventHandler(ISender sender)
    : IntegrationEventHandler<NomenclatureCreatedIntegrationEvent>
{
    public override async Task Handle(
        NomenclatureCreatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        var command = new SyncNomenclatureCommand(
            integrationEvent.NomenclatureId,
            integrationEvent.Code,
            integrationEvent.Name,
            integrationEvent.Description,
            integrationEvent.Category,
            integrationEvent.Unit,
            integrationEvent.MinStockLevel,
            integrationEvent.IsActive);

        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            throw new InvalidOperationException($"Failed to sync nomenclature into Payments module: {result.Error}");
        }
    }
}
