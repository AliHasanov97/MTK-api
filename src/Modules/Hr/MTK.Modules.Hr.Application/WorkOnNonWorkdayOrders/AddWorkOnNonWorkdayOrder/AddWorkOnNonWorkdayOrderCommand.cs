using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.WorkOnNonWorkdayOrders.AddWorkOnNonWorkdayOrder;

public sealed class AddWorkOnNonWorkdayOrderCommand : ICommand<AddWorkOnNonWorkdayOrderResponse>
{
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
}
