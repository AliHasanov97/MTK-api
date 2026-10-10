using MTK.Common.Presentation.Responses;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Hr.Application.OrdersForChangeOfPosition.SearchOrdersForChangeOfPosition;

public sealed class SearchOrdersForChangeOfPositionResponse(
    List<SearchOrdersForChangeOfPositionResponseItem> data,
    int totalCount,
    int page,
    int pageSize)
    : PagedListResponse<SearchOrdersForChangeOfPositionResponseItem>(data, totalCount, page, pageSize);

public sealed class SearchOrdersForChangeOfPositionResponseItem
{
    public Guid Id { get; init; }
    public int OrderNumber { get; init; }
    public Guid ApplicationForChangeOfPositionId { get; init; }
    public ResponseObjectWithName? Employee { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}
