using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Buildings.Events;
using MTK.Modules.Buildings.Domain.Repositories;
using MTK.Modules.Buildings.IntegrationEvents.Buildings;

namespace MTK.Modules.Buildings.Application.Buildings.Events;

internal sealed class BuildingUpdatedDomainEventHandler : DomainEventHandler<BuildingUpdatedDomainEvent>
{
    private readonly IBuildingRepository _buildingRepository;
    private readonly IEventBus _eventBus;

    public BuildingUpdatedDomainEventHandler(IBuildingRepository buildingRepository, IEventBus eventBus)
    {
        _buildingRepository = buildingRepository;
        _eventBus = eventBus;
    }

    public override async Task Handle(
        BuildingUpdatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var building = await _buildingRepository.GetByIdAsync(domainEvent.BuildingId, cancellationToken);
        if (building is null)
        {
            return;
        }

        var integrationEvent = new BuildingUpdatedIntegrationEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            building.Id,
            building.Name,
            building.Address.GetFullAddress());

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
