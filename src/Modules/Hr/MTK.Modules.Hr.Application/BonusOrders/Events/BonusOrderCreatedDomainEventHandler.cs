using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Domain.BonusOrders.Events;
using MTK.Modules.Hr.IntegrationEvents.BonusOrders;

namespace MTK.Modules.Hr.Application.BonusOrders.Events;

internal sealed class BonusOrderCreatedDomainEventHandler : DomainEventHandler<BonusOrderCreatedDomainEvent>
{
    private readonly IEventBus _eventBus;

    public BonusOrderCreatedDomainEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public override async Task Handle(
        BonusOrderCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var integrationEvent = new BonusOrderCreatedIntegrationEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            domainEvent.BonusOrderId,
            domainEvent.EmployeeId,
            domainEvent.BonusAmount,
            domainEvent.SalaryMonth,
            domainEvent.SalaryYear);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
