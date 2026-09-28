using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Modules.Buildings.Domain.OwnershipHistories.Events;
using MTK.Modules.Buildings.IntegrationEvents.OwnershipHistories;

namespace MTK.Modules.Buildings.Application.Apartments.Events;

internal sealed class OwnershipTransferredDomainEventHandler : DomainEventHandler<OwnershipTransferredDomainEvent>
{
    private readonly IEventBus _eventBus;

    public OwnershipTransferredDomainEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public override async Task Handle(
        OwnershipTransferredDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        // Publish integration event to other modules
        var integrationEvent = new OwnershipTransferredIntegrationEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            domainEvent.ApartmentId,
            domainEvent.PreviousOwnerId,
            domainEvent.NewOwnerId,
            domainEvent.TransferDate);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
