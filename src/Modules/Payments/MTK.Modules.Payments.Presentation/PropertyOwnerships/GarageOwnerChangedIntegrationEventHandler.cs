using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.IntegrationEvents.Garages;
using MTK.Modules.Payments.Application.Garages.Commands.SyncGarage;
using MTK.Modules.Payments.Application.PropertyOwnerships.Commands.SyncPropertyOwnership;
using MTK.Modules.Payments.Domain.Charges;
using PaymentsGarageType = MTK.Modules.Payments.Domain.PropertyOwnerships.GarageType;

namespace MTK.Modules.Payments.Presentation.PropertyOwnerships;

internal sealed class GarageOwnerChangedIntegrationEventHandler(ISender sender)
    : IntegrationEventHandler<GarageOwnerChangedIntegrationEvent>
{
    public override async Task Handle(
        GarageOwnerChangedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        PaymentsGarageType garageType = Enum.TryParse<PaymentsGarageType>(integrationEvent.GarageType, ignoreCase: true, out var parsed)
            ? parsed
            : PaymentsGarageType.OpenParking;

        // Keep the descriptive shadow fresh too — this event also carries the
        // garage's current number/type, same as GarageCreated.
        var syncGarage = new SyncGarageCommand(integrationEvent.GarageId, integrationEvent.GarageNumber, garageType);
        Result garageResult = await sender.Send(syncGarage, cancellationToken);
        if (garageResult.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to sync garage shadow: {garageResult.Error}");
        }

        var command = new SyncPropertyOwnershipCommand(
            ApartmentId: null,
            GarageId: integrationEvent.GarageId,
            integrationEvent.NewOwnerId);

        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to sync garage ownership: {result.Error}");
        }
    }
}
