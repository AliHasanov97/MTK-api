using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Nomenclatures.Commands.SyncNomenclature;
using MTK.Modules.Warehouse.IntegrationEvents.Nomenclatures;

namespace MTK.Modules.Payments.Presentation.Nomenclatures;

/// <summary>Warehouse-da nomenklatura dəyişəndə bu moduldakı güzgünü yeniləyir.</summary>
internal sealed class SyncNomenclatureOnUpdatedIntegrationEventHandler(ISender sender)
    : IntegrationEventHandler<NomenclatureUpdatedIntegrationEvent>
{
    public override async Task Handle(
        NomenclatureUpdatedIntegrationEvent integrationEvent,
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
