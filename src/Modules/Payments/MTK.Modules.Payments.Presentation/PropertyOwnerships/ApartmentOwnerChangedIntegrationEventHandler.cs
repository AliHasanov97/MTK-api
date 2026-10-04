using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.IntegrationEvents.Apartments;
using MTK.Modules.Payments.Application.Apartments.Commands.UpdateApartmentDetails;
using MTK.Modules.Payments.Application.PropertyOwnerships.Commands.SyncPropertyOwnership;

namespace MTK.Modules.Payments.Presentation.PropertyOwnerships;

internal sealed class ApartmentOwnerChangedIntegrationEventHandler(ISender sender)
    : IntegrationEventHandler<ApartmentOwnerChangedIntegrationEvent>
{
    public override async Task Handle(
        ApartmentOwnerChangedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        // Descriptive shadow — kept fresh on every ownership transfer too. This event
        // carries no BuildingId (only the building's name/address, for display
        // elsewhere) — so, unlike ApartmentCreated, it must never go through
        // SyncApartmentCommand (that upserts Building too; a placeholder BuildingId
        // there would silently create a bogus Building row). UpdateApartmentDetailsCommand
        // only ever touches the apartment itself.
        var updateApartment = new UpdateApartmentDetailsCommand(
            integrationEvent.ApartmentId, integrationEvent.ApartmentNumber, integrationEvent.AreaSquareMeters);

        Result apartmentResult = await sender.Send(updateApartment, cancellationToken);
        if (apartmentResult.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to update apartment shadow: {apartmentResult.Error}");
        }

        var command = new SyncPropertyOwnershipCommand(
            ApartmentId: integrationEvent.ApartmentId,
            GarageId: null,
            integrationEvent.NewOwnerId);

        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to sync apartment ownership: {result.Error}");
        }
    }
}
