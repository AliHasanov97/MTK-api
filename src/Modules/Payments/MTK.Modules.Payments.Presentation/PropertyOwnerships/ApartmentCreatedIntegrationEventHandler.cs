using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.IntegrationEvents.Apartments;
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
        // Check if property ownership record exists (might have been created by owner assignment event)
        PropertyOwnership? propertyOwnership = await propertyOwnershipRepository
            .GetByPropertyIdAsync(integrationEvent.ApartmentId, cancellationToken);

        // Only sync if record exists (has owner), otherwise wait for owner assignment
        if (propertyOwnership is not null)
        {
            var command = new SyncPropertyOwnershipCommand(
                integrationEvent.ApartmentId,
                PropertyType.Apartment,
                propertyOwnership.OwnerId,
                integrationEvent.AreaSquareMeters,
                PropertyNumber: integrationEvent.ApartmentNumber);

            Result result = await sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                throw new InvalidOperationException(
                    $"Failed to sync apartment area: {result.Error}");
            }
        }
    }
}
