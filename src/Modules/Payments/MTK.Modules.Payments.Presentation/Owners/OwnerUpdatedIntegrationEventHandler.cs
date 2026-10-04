using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.IntegrationEvents.Owners;
using MTK.Modules.Payments.Application.Owners.Commands.SyncOwner;

namespace MTK.Modules.Payments.Presentation.Owners;

internal sealed class OwnerUpdatedIntegrationEventHandler(ISender sender)
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
    }
}
