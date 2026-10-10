using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.WorkOnNonWorkdayOrders.DeleteWorkOnNonWorkdayOrder;

public sealed class DeleteWorkOnNonWorkdayOrderCommand : ICommand
{
    public Guid Id { get; set; }
}
