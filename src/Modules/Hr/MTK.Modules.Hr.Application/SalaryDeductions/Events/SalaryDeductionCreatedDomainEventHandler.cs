using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Domain.SalaryDeductions.Events;
using MTK.Modules.Hr.IntegrationEvents.SalaryDeductions;

namespace MTK.Modules.Hr.Application.SalaryDeductions.Events;

internal sealed class SalaryDeductionCreatedDomainEventHandler : DomainEventHandler<SalaryDeductionCreatedDomainEvent>
{
    private readonly IEventBus _eventBus;

    public SalaryDeductionCreatedDomainEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public override async Task Handle(
        SalaryDeductionCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var integrationEvent = new SalaryDeductionCreatedIntegrationEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            domainEvent.SalaryDeductionId,
            domainEvent.EmployeeId,
            domainEvent.PercentageSalary,
            domainEvent.StartDate);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
