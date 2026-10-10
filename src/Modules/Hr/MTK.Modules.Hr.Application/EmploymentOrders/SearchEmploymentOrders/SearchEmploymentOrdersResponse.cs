using MTK.Common.Presentation.Responses;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Hr.Application.EmploymentOrders.SearchEmploymentOrders;

public class SearchEmploymentOrdersResponse(
    List<SearchEmploymentOrdersResponseItem> data,
    int totalCount,
    int page,
    int pageSize)
    : PagedListResponse<SearchEmploymentOrdersResponseItem>(data, totalCount, page, pageSize);

public class SearchEmploymentOrdersResponseItem
{
    public Guid Id { get; set; }
    public int OrderNumber { get; set; }
    public ResponseObjectWithName JobApplication { get; set; } = null!;
    public ResponseObjectWithName Employee { get; set; } = null!;
    public ResponseObjectWithName Job { get; set; } = null!;
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public ResponseObjectWithName? LaborCodeCase { get; set; }
    public ResponseObjectWithName CreatedBy { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
