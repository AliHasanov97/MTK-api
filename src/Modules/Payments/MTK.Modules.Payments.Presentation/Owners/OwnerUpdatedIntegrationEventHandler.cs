using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.IntegrationEvents.Owners;
using MTK.Modules.Payments.Application.Owners.Commands.SyncOwner;
using MTK.Modules.Payments.Application.Owners.Services;

namespace MTK.Modules.Payments.Presentation.Owners;

internal sealed class OwnerUpdatedIntegrationEventHandler(
    ISender sender,
    ICachedOwnerService cachedOwnerService)
    : IntegrationEventHandler<OwnerUpdatedIntegrationEvent>
{
    public override async Task Handle(
        OwnerUpdatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        var command = new SyncOwnerCommand(integrationEvent.OwnerId, integrationEvent.FullName);
        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            throw new InvalidOperationException($"Failed to sync owner: {result.Error}");
        }

        // Invalidate Owner cache after owner update (FullName is in cache!)
        await cachedOwnerService.InvalidateCacheAsync(integrationEvent.OwnerId, cancellationToken);
    }
}
