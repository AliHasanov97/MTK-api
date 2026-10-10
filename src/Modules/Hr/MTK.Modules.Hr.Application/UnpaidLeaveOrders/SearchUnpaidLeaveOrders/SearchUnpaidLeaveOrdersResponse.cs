using MTK.Common.Presentation.Responses;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Hr.Application.UnpaidLeaveOrders.SearchUnpaidLeaveOrders;

public class SearchUnpaidLeaveOrdersResponse(
    List<SearchUnpaidLeaveOrdersResponseItem> data,
    int totalCount,
    int page,
    int pageSize)
    : PagedListResponse<SearchUnpaidLeaveOrdersResponseItem>(data, totalCount, page, pageSize);

public class SearchUnpaidLeaveOrdersResponseItem
{
    public Guid Id { get; set; }
    public int OrderNumber { get; set; }
    public ResponseObjectWithName Employee { get; set; } = null!;
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public ResponseObjectWithName CreatedBy { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
