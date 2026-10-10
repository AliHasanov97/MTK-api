using MTK.Common.Presentation.Responses;

namespace MTK.Modules.Hr.Application.WorkOnNonWorkdayOrders.GetWorkOnNonWorkdayOrderById;

public sealed class GetWorkOnNonWorkdayOrderByIdResponse
{
    public Guid Id { get; set; }
    public int OrderNumber { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public ResponseObjectWithName CreatedBy { get; set; } = null!;
}
