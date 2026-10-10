using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.WorkOnNonWorkdayOrders.GetWorkOnNonWorkdayOrderById;

public sealed class GetWorkOnNonWorkdayOrderByIdQuery : IQuery<GetWorkOnNonWorkdayOrderByIdResponse>
{
    public Guid Id { get; set; }
}
