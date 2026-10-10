using MTK.Common.Presentation.Responses;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Hr.Application.EmploymentStatusChangeOrders.SearchEmploymentStatusChangeOrders;

public class SearchEmploymentStatusChangeOrdersResponse(
    List<SearchEmploymentStatusChangeOrdersResponseItem> data,
    int totalCount,
    int page,
    int pageSize)
    : PagedListResponse<SearchEmploymentStatusChangeOrdersResponseItem>(data, totalCount, page, pageSize);

public class SearchEmploymentStatusChangeOrdersResponseItem
{
    public Guid Id { get; set; }
    public int OrderNumber { get; set; }
    public ResponseObjectWithName Employee { get; set; } = null!;
    public string CurrentEmploymentType { get; set; } = null!;
    public string NewEmploymentType { get; set; } = null!;
    public ResponseObjectWithName OrderExecutionSupervisor { get; set; } = null!;
    public ResponseObjectWithName CreatedBy { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
