using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.IntegrationEvents.Garages;
using MTK.Modules.Payments.Application.PropertyOwnerships.Commands.SyncPropertyOwnership;
using MTK.Modules.Payments.Domain.Charges;
using PaymentsGarageType = MTK.Modules.Payments.Domain.PropertyOwnerships.GarageType;

namespace MTK.Modules.Payments.Presentation.PropertyOwnerships;

internal sealed class GarageCreatedIntegrationEventHandler(ISender sender)
    : IntegrationEventHandler<GarageCreatedIntegrationEvent>
{
    public override async Task Handle(
        GarageCreatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        // Only sync if garage has an owner
        if (integrationEvent.OwnerId.HasValue)
        {
            PaymentsGarageType? garageType = Enum.TryParse<PaymentsGarageType>(integrationEvent.GarageType, ignoreCase: true, out var parsed)
                ? parsed
                : null;

            var command = new SyncPropertyOwnershipCommand(
                integrationEvent.GarageId,
                PropertyType.Garage,
                integrationEvent.OwnerId.Value,
                0, // Garages use fixed rate, not area-based
                garageType,
                PropertyNumber: integrationEvent.GarageNumber);

            Result result = await sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                throw new InvalidOperationException(
                    $"Failed to sync garage creation: {result.Error}");
            }
        }
    }
}
