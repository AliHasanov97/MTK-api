using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Modules.Buildings.Domain.Owners.Events;
using MTK.Modules.Buildings.IntegrationEvents.Owners;

namespace MTK.Modules.Buildings.Application.Owners.Events;

internal sealed class OwnerContactInfoUpdatedDomainEventHandler : DomainEventHandler<OwnerContactInfoUpdatedDomainEvent>
{
    private readonly IEventBus _eventBus;

    public OwnerContactInfoUpdatedDomainEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public override async Task Handle(
        OwnerContactInfoUpdatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var integrationEvent = new OwnerUpdatedIntegrationEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            domainEvent.OwnerId,
            domainEvent.FullName);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
