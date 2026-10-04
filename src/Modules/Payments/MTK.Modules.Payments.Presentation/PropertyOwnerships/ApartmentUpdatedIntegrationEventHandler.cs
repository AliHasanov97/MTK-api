using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.IntegrationEvents.Apartments;
using MTK.Modules.Payments.Application.Apartments.Commands.UpdateApartmentDetails;

namespace MTK.Modules.Payments.Presentation.PropertyOwnerships;

internal sealed class ApartmentUpdatedIntegrationEventHandler(ISender sender)
    : IntegrationEventHandler<ApartmentUpdatedIntegrationEvent>
{
    public override async Task Handle(
        ApartmentUpdatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        // Keeps the Apartment shadow's own area current — PropertyOwnership no longer
        // carries a copy of it. This event has no ApartmentNumber/BuildingId, so the
        // number is left as-is and Building is never touched (see
        // UpdateApartmentDetailsCommand — unlike SyncApartmentCommand).
        var command = new UpdateApartmentDetailsCommand(
            integrationEvent.ApartmentId, ApartmentNumber: null, integrationEvent.AreaSquareMeters);
        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            throw new InvalidOperationException($"Failed to update apartment details: {result.Error}");
        }
    }
}
