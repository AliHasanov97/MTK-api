using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.IntegrationEvents.OwnershipHistories;
using MTK.Modules.Payments.Application.PropertyOwnerships.Commands.SyncPropertyOwnership;
using MTK.Modules.Payments.Domain.Charges;

namespace MTK.Modules.Payments.Presentation.PropertyOwnerships;

/// <summary>
/// Mənzil transferi zamanı PropertyOwnership-i yeni owner-ə update edir
/// </summary>
internal sealed class OwnershipTransferredIntegrationEventHandler(ISender sender)
    : IntegrationEventHandler<OwnershipTransferredIntegrationEvent>
{
    public override async Task Handle(
        OwnershipTransferredIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        // Transfer zamanı PropertyOwnership-i yeni owner-ə update edirik
        var command = new SyncPropertyOwnershipCommand(
            integrationEvent.ApartmentId,
            PropertyType.Apartment,
            integrationEvent.NewOwnerId,
            0); // Area information will be preserved/updated separately

        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to sync ownership transfer: {result.Error}");
        }
    }
}
