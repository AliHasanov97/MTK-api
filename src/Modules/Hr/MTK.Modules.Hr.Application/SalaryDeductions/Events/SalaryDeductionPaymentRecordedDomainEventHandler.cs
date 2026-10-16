using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Domain.SalaryDeductions.Events;
using MTK.Modules.Hr.IntegrationEvents.SalaryDeductions;

namespace MTK.Modules.Hr.Application.SalaryDeductions.Events;

internal sealed class SalaryDeductionPaymentRecordedDomainEventHandler
    : DomainEventHandler<SalaryDeductionPaymentRecordedDomainEvent>
{
    private readonly IEventBus _eventBus;

    public SalaryDeductionPaymentRecordedDomainEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public override async Task Handle(
        SalaryDeductionPaymentRecordedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        // RemainingDebt sync et (Payments moduluna)
        var integrationEvent = new SalaryDeductionUpdatedIntegrationEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            domainEvent.SalaryDeductionId,
            domainEvent.EmployeeId,
            domainEvent.RemainingDebt);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
