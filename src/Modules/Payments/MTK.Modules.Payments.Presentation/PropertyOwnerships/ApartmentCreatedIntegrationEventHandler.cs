using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.IntegrationEvents.Apartments;
using MTK.Modules.Payments.Application.Apartments.Commands.SyncApartment;
using MTK.Modules.Payments.Application.PropertyOwnerships.Commands.SyncPropertyOwnership;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.PropertyOwnerships;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Presentation.PropertyOwnerships;

internal sealed class ApartmentCreatedIntegrationEventHandler(
    ISender sender,
    IPropertyOwnershipRepository propertyOwnershipRepository)
    : IntegrationEventHandler<ApartmentCreatedIntegrationEvent>
{
    public override async Task Handle(
        ApartmentCreatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        // Descriptive shadow (what/where this apartment is) — independent of
        // ownership, always kept fresh regardless of whether an owner exists yet.
        var syncApartment = new SyncApartmentCommand(
            integrationEvent.ApartmentId,
            integrationEvent.BuildingId,
            integrationEvent.ApartmentNumber,
            integrationEvent.AreaSquareMeters,
            integrationEvent.BuildingName,
            integrationEvent.BuildingAddress);

        Result apartmentResult = await sender.Send(syncApartment, cancellationToken);
        if (apartmentResult.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to sync apartment shadow: {apartmentResult.Error}");
        }

        // Check if property ownership record exists (might have been created by owner assignment event)
        PropertyOwnership? propertyOwnership = await propertyOwnershipRepository
            .GetByPropertyIdAsync(integrationEvent.ApartmentId, cancellationToken);

        // Only sync if record exists (has owner), otherwise wait for owner assignment
        if (propertyOwnership is not null)
        {
            var command = new SyncPropertyOwnershipCommand(
                ApartmentId: integrationEvent.ApartmentId,
                GarageId: null,
                propertyOwnership.OwnerId);

            Result result = await sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                throw new InvalidOperationException(
                    $"Failed to sync apartment area: {result.Error}");
            }
        }
    }
}
