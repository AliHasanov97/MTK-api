using MTK.Common.Application.EventBus;

namespace MTK.Modules.Hr.IntegrationEvents.Payroll;

/// <summary>
/// İş günü olmayan gündə işə cəlb etmə əmri yaradıldıqda yayımlanır. Bu əmrdə işçi
/// bağlantısı (EmployeeId) yoxdur — buna görə yalnız bildirişdir və hazırda heç bir
/// modul onu əmək haqqı hesablamasına qoşmur (gələcəkdə işçi bağlananda genişləndirilə bilər).
/// </summary>
public sealed class WorkOnNonWorkdayOrderCreatedIntegrationEvent : IntegrationEvent
{
    public WorkOnNonWorkdayOrderCreatedIntegrationEvent(
        Guid integrationEventId,
        DateTime occurredOnUtc,
        Guid orderId,
        DateTimeOffset startDate,
        DateTimeOffset? endDate)
        : base(integrationEventId, occurredOnUtc)
    {
        OrderId = orderId;
        StartDate = startDate;
        EndDate = endDate;
    }

    public Guid OrderId { get; }

    public DateTimeOffset StartDate { get; }

    public DateTimeOffset? EndDate { get; }
}
