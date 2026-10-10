using MTK.Common.Presentation.Responses;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Hr.Application.VacationReturnOrders.SearchVacationReturnOrders;

public class SearchVacationReturnOrdersResponse(
    List<SearchVacationReturnOrdersResponseItem> data,
    int totalCount,
    int page,
    int pageSize)
    : PagedListResponse<SearchVacationReturnOrdersResponseItem>(data, totalCount, page, pageSize);

public class SearchVacationReturnOrdersResponseItem
{
    public Guid Id { get; set; }
    public int OrderNumber { get; set; }
    public ResponseObjectWithName Employee { get; set; } = null!;
    public DateTimeOffset ReturnDate { get; set; }
    public ResponseObjectWithName CreatedBy { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
