using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Domain.SalaryDeductions.Events;
using MTK.Modules.Hr.IntegrationEvents.SalaryDeductions;

namespace MTK.Modules.Hr.Application.SalaryDeductions.Events;

internal sealed class SalaryDeductionDeletedDomainEventHandler : DomainEventHandler<SalaryDeductionDeletedDomainEvent>
{
    private readonly IEventBus _eventBus;

    public SalaryDeductionDeletedDomainEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public override async Task Handle(
        SalaryDeductionDeletedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var integrationEvent = new SalaryDeductionDeletedIntegrationEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            domainEvent.SalaryDeductionId,
            domainEvent.EmployeeId);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
