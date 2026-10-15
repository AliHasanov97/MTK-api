using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Domain.VacationOrders.Events;
using MTK.Modules.Hr.IntegrationEvents.Payroll;

namespace MTK.Modules.Hr.Application.VacationOrders.Events;

/// <summary>
/// Ödənişli məzuniyyət əmri. İşçinin aylıq maaşı məzuniyyət zamanı saxlanılır — buna görə
/// maliyyə təsiri yoxdur (0 gün). Lakin əvvəllər heç bir modul bu əmrdən xəbər tutmurdu;
/// axının tamamlanması və gələcək hesablamalar üçün informasiya kimi yayımlanır.
/// </summary>
internal sealed class VacationOrderCreatedDomainEventHandler : DomainEventHandler<VacationOrderCreatedDomainEvent>
{
    private readonly IEventBus _eventBus;

    public VacationOrderCreatedDomainEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public override async Task Handle(
        VacationOrderCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var effectiveDate = domainEvent.StartDate.UtcDateTime;

        var integrationEvent = new PayrollAdjustmentRequestedIntegrationEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            domainEvent.OrderId,
            domainEvent.EmployeeId,
            PayrollAdjustmentReason.Vacation,
            hours: 0m,
            normHours: 0m,
            salaryMonth: effectiveDate.Month,
            salaryYear: effectiveDate.Year);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
