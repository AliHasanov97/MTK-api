using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Domain.Companies.Events;
using MTK.Modules.Payments.IntegrationEvents.Companies;

namespace MTK.Modules.Payments.Application.Companies.Events;

internal sealed class CompanyCreatedDomainEventHandler : DomainEventHandler<CompanyCreatedDomainEvent>
{
    private readonly IEventBus _eventBus;

    public CompanyCreatedDomainEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public override async Task Handle(CompanyCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var integrationEvent = new CompanyCreatedIntegrationEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            domainEvent.CompanyId,
            domainEvent.Name,
            domainEvent.Director,
            domainEvent.Address);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
