using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Domain.Payroll.Events;
using MTK.Modules.Payments.IntegrationEvents.Payroll;

namespace MTK.Modules.Payments.Application.Payroll.Events;

internal sealed class PayslipPaidDomainEventHandler : DomainEventHandler<PayslipPaidDomainEvent>
{
    private readonly IEventBus _eventBus;

    public PayslipPaidDomainEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public override async Task Handle(
        PayslipPaidDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var integrationEvent = new PayrollPaymentCompletedIntegrationEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            domainEvent.PayslipId,
            domainEvent.EmployeeId,
            domainEvent.CourtDeductionAmount,
            domainEvent.SalaryMonth,
            domainEvent.SalaryYear);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
