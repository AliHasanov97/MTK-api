using MTK.Common.Presentation.Responses;
using MTK.Common.Domain.Queries;
using MTK.Modules.Hr.Domain.Orders;

namespace MTK.Modules.Hr.Application.Orders.SearchOrders;

public sealed class SearchOrdersResponse(
    List<SearchOrdersResponseItem> data,
    int totalCount,
    int page,
    int pageSize)
    : PagedListResponse<SearchOrdersResponseItem>(data, totalCount, page, pageSize);

public sealed class SearchOrdersResponseItem
{
    public Guid Id { get; init; }
    public int OrderNumber { get; init; }
    public OrderType Type { get; init; }
    public ResponseObjectWithName? Employee { get; init; }
    public ResponseObjectWithName? JobApplicant { get; init; }
    public ResponseObjectWithName? CreatedBy { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}