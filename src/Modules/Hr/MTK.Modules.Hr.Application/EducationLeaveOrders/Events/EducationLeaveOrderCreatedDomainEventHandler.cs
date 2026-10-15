using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Domain.EducationLeaveOrders.Events;
using MTK.Modules.Hr.IntegrationEvents.Payroll;

namespace MTK.Modules.Hr.Application.EducationLeaveOrders.Events;

/// <summary>
/// Təhsil məzuniyyəti əmri. Qanunvericiliyə əsasən təhsil məzuniyyəti ödənişli ola bilər —
/// buna görə avtomatik maaş kəsilməsi tətbiq edilmir (0 gün), əmr informasiya kimi bildirilir.
/// </summary>
internal sealed class EducationLeaveOrderCreatedDomainEventHandler
    : DomainEventHandler<EducationLeaveOrderCreatedDomainEvent>
{
    private readonly IEventBus _eventBus;

    public EducationLeaveOrderCreatedDomainEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public override async Task Handle(
        EducationLeaveOrderCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var effectiveDate = domainEvent.StartDate.UtcDateTime;

        var integrationEvent = new PayrollAdjustmentRequestedIntegrationEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            domainEvent.OrderId,
            domainEvent.EmployeeId,
            PayrollAdjustmentReason.EducationLeave,
            hours: 0m,
            normHours: 0m,
            salaryMonth: effectiveDate.Month,
            salaryYear: effectiveDate.Year);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
