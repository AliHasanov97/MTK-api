using MTK.Common.Presentation.Responses;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Hr.Application.VacationOrders.SearchVacationOrders;

public class SearchVacationOrdersResponse(
    List<SearchVacationOrdersResponseItem> data,
    int totalCount,
    int page,
    int pageSize)
    : PagedListResponse<SearchVacationOrdersResponseItem>(data, totalCount, page, pageSize);

public class SearchVacationOrdersResponseItem
{
    public Guid Id { get; set; }
    public int OrderNumber { get; set; }
    public Guid VacationApplicationId { get; set; }
    public ResponseObjectWithName Employee { get; set; } = null!;
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public int VacationDays { get; set; }
    public ResponseObjectWithName CreatedBy { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}