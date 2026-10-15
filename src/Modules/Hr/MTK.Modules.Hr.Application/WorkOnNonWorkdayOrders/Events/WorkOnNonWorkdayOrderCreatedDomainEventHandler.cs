using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Domain.WorkOnNonWorkdayOrders.Events;
using MTK.Modules.Hr.IntegrationEvents.Payroll;

namespace MTK.Modules.Hr.Application.WorkOnNonWorkdayOrders.Events;

/// <summary>
/// İş günü olmayan gündə işə cəlb etmə əmri. Bu əmrdə işçi bağlantısı yoxdur — buna görə
/// yalnız bildiriş kimi yayımlanır (hazırda heç bir modul əmək haqqına qoşmur).
/// </summary>
internal sealed class WorkOnNonWorkdayOrderCreatedDomainEventHandler
    : DomainEventHandler<WorkOnNonWorkdayOrderCreatedDomainEvent>
{
    private readonly IEventBus _eventBus;

    public WorkOnNonWorkdayOrderCreatedDomainEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public override async Task Handle(
        WorkOnNonWorkdayOrderCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var integrationEvent = new WorkOnNonWorkdayOrderCreatedIntegrationEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            domainEvent.OrderId,
            domainEvent.StartDate,
            domainEvent.EndDate);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
