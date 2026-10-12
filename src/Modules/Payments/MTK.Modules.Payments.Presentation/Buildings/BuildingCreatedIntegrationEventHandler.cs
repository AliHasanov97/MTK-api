using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.IntegrationEvents.Buildings;
using MTK.Modules.Payments.Application.Buildings.Commands.SyncBuilding;
using MTK.Modules.Payments.Application.Buildings.Services;

namespace MTK.Modules.Payments.Presentation.Buildings;

internal sealed class BuildingCreatedIntegrationEventHandler(
    ISender sender,
    ICachedBuildingService cachedBuildingService)
    : IntegrationEventHandler<BuildingCreatedIntegrationEvent>
{
    public override async Task Handle(
        BuildingCreatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        var command = new SyncBuildingCommand(integrationEvent.BuildingId, integrationEvent.Name, integrationEvent.Address);
        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            throw new InvalidOperationException($"Failed to sync building shadow: {result.Error}");
        }

        // Invalidate Building cache after building creation
        await cachedBuildingService.InvalidateCacheAsync(integrationEvent.BuildingId, cancellationToken);
    }
}
