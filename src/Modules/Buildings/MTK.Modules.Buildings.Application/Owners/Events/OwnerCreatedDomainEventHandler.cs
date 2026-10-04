using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Modules.Buildings.Domain.Owners.Events;
using MTK.Modules.Buildings.IntegrationEvents.Owners;

namespace MTK.Modules.Buildings.Application.Owners.Events;

internal sealed class OwnerCreatedDomainEventHandler : DomainEventHandler<OwnerCreatedDomainEvent>
{
    private readonly IEventBus _eventBus;

    public OwnerCreatedDomainEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public override async Task Handle(
        OwnerCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        // The domain event already carries everything needed — no repository lookup required.
        var integrationEvent = new OwnerCreatedIntegrationEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            domainEvent.OwnerId,
            domainEvent.FullName);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
