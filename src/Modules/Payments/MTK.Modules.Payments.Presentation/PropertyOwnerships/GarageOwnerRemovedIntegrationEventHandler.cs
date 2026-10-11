using MediatR;
using MTK.Modules.Payments.Application.PropertyOwnerships.Services;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.IntegrationEvents.Garages;
using MTK.Modules.Payments.Application.PropertyOwnerships.Commands.RemovePropertyOwnership;

namespace MTK.Modules.Payments.Presentation.PropertyOwnerships;

internal sealed class GarageOwnerRemovedIntegrationEventHandler(
    ISender sender,
    ICachedPropertyOwnershipService cachedPropertyOwnershipService)
    : IntegrationEventHandler<GarageOwnerRemovedIntegrationEvent>
{
    public override async Task Handle(
        GarageOwnerRemovedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        var command = new RemovePropertyOwnershipCommand(integrationEvent.GarageId);

        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to remove garage ownership: {result.Error}");
        }

        // Invalidate PropertyOwnership cache after property ownership change
        await cachedPropertyOwnershipService.InvalidateCacheAsync(cancellationToken);
    }
}
