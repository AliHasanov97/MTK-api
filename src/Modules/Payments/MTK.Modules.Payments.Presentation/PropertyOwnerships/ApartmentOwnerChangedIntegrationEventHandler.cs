using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.IntegrationEvents.Apartments;
using MTK.Modules.Payments.Application.PropertyOwnerships.Commands.SyncPropertyOwnership;
using MTK.Modules.Payments.Domain.Charges;

namespace MTK.Modules.Payments.Presentation.PropertyOwnerships;

internal sealed class ApartmentOwnerChangedIntegrationEventHandler(ISender sender)
    : IntegrationEventHandler<ApartmentOwnerChangedIntegrationEvent>
{
    public override async Task Handle(
        ApartmentOwnerChangedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        var command = new SyncPropertyOwnershipCommand(
            integrationEvent.ApartmentId,
            PropertyType.Apartment,
            integrationEvent.NewOwnerId,
            integrationEvent.AreaSquareMeters);

        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to sync apartment ownership: {result.Error}");
        }
    }
}
