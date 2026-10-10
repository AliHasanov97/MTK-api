using MTK.Common.Presentation.Responses;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Hr.Application.CompensationOrders.SearchCompensationOrders;

public class SearchCompensationOrdersResponse(
    List<SearchCompensationOrdersResponseItem> data,
    int totalCount,
    int page,
    int pageSize)
    : PagedListResponse<SearchCompensationOrdersResponseItem>(data, totalCount, page, pageSize);

public class SearchCompensationOrdersResponseItem
{
    public Guid Id { get; set; }
    public int OrderNumber { get; set; }
    public ResponseObjectWithName Employee { get; set; } = null!;
    public int CompensatedDays { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public ResponseObjectWithName CreatedBy { get; set; } = null!;
}