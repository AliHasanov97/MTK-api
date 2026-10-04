using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Buildings.Events;
using MTK.Modules.Buildings.Domain.Repositories;
using MTK.Modules.Buildings.IntegrationEvents.Buildings;

namespace MTK.Modules.Buildings.Application.Buildings.Events;

internal sealed class BuildingCreatedDomainEventHandler : DomainEventHandler<BuildingCreatedDomainEvent>
{
    private readonly IBuildingRepository _buildingRepository;
    private readonly IEventBus _eventBus;

    public BuildingCreatedDomainEventHandler(IBuildingRepository buildingRepository, IEventBus eventBus)
    {
        _buildingRepository = buildingRepository;
        _eventBus = eventBus;
    }

    public override async Task Handle(
        BuildingCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        // Domain event only carries Name — Address lives on the entity, re-fetched here.
        var building = await _buildingRepository.GetByIdAsync(domainEvent.BuildingId, cancellationToken);
        if (building is null)
        {
            return;
        }

        var integrationEvent = new BuildingCreatedIntegrationEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            building.Id,
            building.Name,
            building.Address.GetFullAddress());

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
