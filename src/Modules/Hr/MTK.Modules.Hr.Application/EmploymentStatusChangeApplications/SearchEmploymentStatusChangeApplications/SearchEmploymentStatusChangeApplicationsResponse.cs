using MTK.Common.Presentation.Responses;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Hr.Application.EmploymentStatusChangeApplications.SearchEmploymentStatusChangeApplications;

public class SearchEmploymentStatusChangeApplicationsResponse(
    List<SearchEmploymentStatusChangeApplicationsResponseItem> data,
    int totalCount,
    int page,
    int pageSize)
    : PagedListResponse<SearchEmploymentStatusChangeApplicationsResponseItem>(data, totalCount, page, pageSize);

public class SearchEmploymentStatusChangeApplicationsResponseItem
{
    public Guid Id { get; set; }
    public int ApplicationNumber { get; set; }
    public string Status { get; set; } = null!;
    public ResponseObjectWithName Employee { get; set; } = null!;
    public string CurrentEmploymentType { get; set; } = null!;
    public string NewEmploymentType { get; set; } = null!;
    public ResponseObjectWithName OrderExecutionSupervisor { get; set; } = null!;
    public ResponseObjectWithName CreatedBy { get; set; } = null!;
    public ResponseObjectWithName? Order { get; init; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
