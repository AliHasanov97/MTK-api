using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.IntegrationEvents.Garages;
using MTK.Modules.Payments.Application.Garages.Commands.SyncGarage;
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
        PaymentsGarageType garageType = Enum.TryParse<PaymentsGarageType>(integrationEvent.GarageType, ignoreCase: true, out var parsed)
            ? parsed
            : PaymentsGarageType.OpenParking;

        // Descriptive shadow (what this garage concretely is) — independent of
        // ownership, always kept fresh regardless of whether an owner exists yet.
        var syncGarage = new SyncGarageCommand(integrationEvent.GarageId, integrationEvent.GarageNumber, garageType);
        Result garageResult = await sender.Send(syncGarage, cancellationToken);
        if (garageResult.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to sync garage shadow: {garageResult.Error}");
        }

        // Only sync ownership if garage has an owner
        if (integrationEvent.OwnerId.HasValue)
        {
            var command = new SyncPropertyOwnershipCommand(
                ApartmentId: null,
                GarageId: integrationEvent.GarageId,
                integrationEvent.OwnerId.Value);

            Result result = await sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                throw new InvalidOperationException(
                    $"Failed to sync garage creation: {result.Error}");
            }
        }
    }
}
