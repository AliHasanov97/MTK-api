using MTK.Modules.Hr.Domain.Orders;

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

        return workOnNonWorkdayOrder;
    }
}
