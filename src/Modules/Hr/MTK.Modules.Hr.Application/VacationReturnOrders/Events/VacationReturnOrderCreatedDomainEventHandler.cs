using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Domain.VacationReturnOrders.Events;
using MTK.Modules.Hr.IntegrationEvents.Payroll;

namespace MTK.Modules.Hr.Application.VacationReturnOrders.Events;

/// <summary>
/// Məzuniyyətdən geri qayıtma əmri. Ödənişli məzuniyyətin kəsilməsidir — aylıq maaşa birbaşa
/// maliyyə təsiri yoxdur (0 gün), lakin əvvəlki məzuniyyət düzəlişlərinin izlənməsi üçün bildirilir.
/// </summary>
internal sealed class VacationReturnOrderCreatedDomainEventHandler
    : DomainEventHandler<VacationReturnOrderCreatedDomainEvent>
{
    private readonly IEventBus _eventBus;

    public VacationReturnOrderCreatedDomainEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public override async Task Handle(
        VacationReturnOrderCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var effectiveDate = domainEvent.ReturnDate.UtcDateTime;

        var integrationEvent = new PayrollAdjustmentRequestedIntegrationEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            domainEvent.OrderId,
            domainEvent.EmployeeId,
            PayrollAdjustmentReason.VacationReturn,
            hours: 0m,
            normHours: 0m,
            salaryMonth: effectiveDate.Month,
            salaryYear: effectiveDate.Year);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
