using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.IntegrationEvents.Apartments;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.PropertyOwnerships;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Presentation.PropertyOwnerships;

internal sealed class ApartmentUpdatedIntegrationEventHandler(
    IPropertyOwnershipRepository propertyOwnershipRepository,
    IUnitOfWork unitOfWork)
    : IntegrationEventHandler<ApartmentUpdatedIntegrationEvent>
{
    public override async Task Handle(
        ApartmentUpdatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        // Find existing property ownership record
        PropertyOwnership? propertyOwnership = await propertyOwnershipRepository
            .GetByPropertyIdAsync(integrationEvent.ApartmentId, cancellationToken);

        if (propertyOwnership is not null)
        {
            // Update area
            propertyOwnership.UpdateArea(integrationEvent.AreaSquareMeters);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        // If not found, ignore - apartment doesn't have owner yet
    }
}
