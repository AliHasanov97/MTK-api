using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Garages.Events;
using MTK.Modules.Buildings.Domain.Repositories;
using MTK.Modules.Buildings.IntegrationEvents.Garages;

namespace MTK.Modules.Buildings.Application.Garages.Events;

internal sealed class GarageOwnerAssignedDomainEventHandler : DomainEventHandler<GarageOwnerAssignedDomainEvent>
{
    private readonly IGarageRepository _garageRepository;
    private readonly IEventBus _eventBus;

    public GarageOwnerAssignedDomainEventHandler(
        IGarageRepository garageRepository,
        IEventBus eventBus)
    {
        _garageRepository = garageRepository;
        _eventBus = eventBus;
    }

    public override async Task Handle(
        GarageOwnerAssignedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        // Get garage to retrieve full details
        var garage = await _garageRepository.GetByIdAsync(
            domainEvent.GarageId,
            cancellationToken);

        if (garage is null)
        {
            return; // Garage not found, skip
        }

        // Publish integration event to other modules
        var integrationEvent = new GarageOwnerChangedIntegrationEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            garage.Id,
            domainEvent.NewOwnerId,
            domainEvent.PreviousOwnerId,
            garage.GarageNumber,
            garage.Type.ToString());

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
