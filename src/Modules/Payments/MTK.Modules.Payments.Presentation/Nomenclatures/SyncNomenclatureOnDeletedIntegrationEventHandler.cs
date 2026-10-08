using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Nomenclatures.Commands.DeactivateNomenclature;
using MTK.Modules.Warehouse.IntegrationEvents.Nomenclatures;

namespace MTK.Modules.Payments.Presentation.Nomenclatures;

/// <summary>
/// Warehouse-da nomenklatura silinəndə bu moduldakı güzgünü deaktiv edir (sətir qalır —
/// keçmiş alış sətirləri hələ də ona istinad edir).
/// </summary>
internal sealed class SyncNomenclatureOnDeletedIntegrationEventHandler(ISender sender)
    : IntegrationEventHandler<NomenclatureDeletedIntegrationEvent>
{
    public override async Task Handle(
        NomenclatureDeletedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        var command = new DeactivateNomenclatureCommand(integrationEvent.NomenclatureId);

        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            throw new InvalidOperationException($"Failed to deactivate nomenclature in Payments module: {result.Error}");
        }
    }
}
