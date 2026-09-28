using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.IntegrationEvents.Garages;
using MTK.Modules.Payments.Application.PropertyOwnerships.Commands.SyncPropertyOwnership;
using MTK.Modules.Payments.Domain.Charges;

namespace MTK.Modules.Payments.Presentation.PropertyOwnerships;

internal sealed class GarageOwnerChangedIntegrationEventHandler(ISender sender)
    : IntegrationEventHandler<GarageOwnerChangedIntegrationEvent>
{
    public override async Task Handle(
        GarageOwnerChangedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        var command = new SyncPropertyOwnershipCommand(
            integrationEvent.GarageId,
            PropertyType.Garage,
            integrationEvent.NewOwnerId,
            0); // Garages use fixed rate, not area-based

        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to sync garage ownership: {result.Error}");
        }
    }
}
