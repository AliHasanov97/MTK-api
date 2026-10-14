using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Domain.Companies.Events;
using MTK.Modules.Payments.IntegrationEvents.Companies;

namespace MTK.Modules.Payments.Application.Companies.Events;

internal sealed class CompanyUpdatedDomainEventHandler : DomainEventHandler<CompanyUpdatedDomainEvent>
{
    private readonly IEventBus _eventBus;

    public CompanyUpdatedDomainEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public override async Task Handle(CompanyUpdatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var integrationEvent = new CompanyUpdatedIntegrationEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            domainEvent.CompanyId,
            domainEvent.Name,
            domainEvent.Director,
            domainEvent.Address);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
