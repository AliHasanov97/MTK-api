using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Domain.Employees.Events;
using MTK.Modules.Hr.IntegrationEvents.Employees;

namespace MTK.Modules.Hr.Application.Employees.Events;

internal sealed class EmployeeUpdatedDomainEventHandler : DomainEventHandler<EmployeeUpdatedDomainEvent>
{
    private readonly IEventBus _eventBus;

    public EmployeeUpdatedDomainEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public override async Task Handle(
        EmployeeUpdatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        // The domain event already carries everything needed — no repository lookup required.
        var integrationEvent = new EmployeeUpdatedIntegrationEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            domainEvent.EmployeeId,
            domainEvent.FullName,
            domainEvent.BaseSalary);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
