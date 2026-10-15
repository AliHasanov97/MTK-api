using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.WorkOnNonWorkdayOrders.Events;

/// <summary>
/// İş günü olmayan gündə işə cəlb etmə əmri yaradıldıqda qalxır. Bu, əlavə ödəniş
/// tələb edən əmrdir, lakin hazırkı modeldə işçi bağlantısı (EmployeeId) yoxdur —
/// buna görə yalnız bildiriş kimi yayımlanır, gələcəkdə işçi bağlananda payroll-a qoşula bilər.
/// </summary>
public sealed record WorkOnNonWorkdayOrderCreatedDomainEvent : DomainEvent
{
    public required Guid OrderId { get; init; }
    public required DateTimeOffset StartDate { get; init; }
    public DateTimeOffset? EndDate { get; init; }
}
