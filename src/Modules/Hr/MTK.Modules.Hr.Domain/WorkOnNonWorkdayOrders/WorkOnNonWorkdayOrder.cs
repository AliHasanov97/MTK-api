using MTK.Modules.Hr.Domain.Orders;
using MTK.Modules.Hr.Domain.WorkOnNonWorkdayOrders.Events;

namespace MTK.Modules.Hr.Domain.WorkOnNonWorkdayOrders;

public sealed class WorkOnNonWorkdayOrder : Order
{
    public DateTimeOffset StartDate { get; private set; }
    public DateTimeOffset? EndDate { get; private set; }

    private WorkOnNonWorkdayOrder() { }

    public static WorkOnNonWorkdayOrder Create(
        DateTimeOffset startDate,
        DateTimeOffset? endDate,
        Guid createdById)
    {
        var workOnNonWorkdayOrder = new WorkOnNonWorkdayOrder
        {
            Id = Guid.NewGuid(),
            Type = OrderType.WorkOnNonWorkday,
            StartDate = startDate,
            EndDate = endDate,
            CreatedById = createdById
        };

        // İş günü olmayan gündə iş əlavə ödəniş tələb edir, lakin bu əmrdə işçi sahəsi yoxdur —
        // buna görə yalnız bildiriş kimi yayımlanır (bax: WorkOnNonWorkdayOrderCreatedDomainEventHandler).
        workOnNonWorkdayOrder.RaiseDomainEvent(new WorkOnNonWorkdayOrderCreatedDomainEvent
        {
            OrderId = workOnNonWorkdayOrder.Id,
            StartDate = startDate,
            EndDate = endDate
        });

        return workOnNonWorkdayOrder;
    }
}
