using MTK.Common.Presentation.Responses;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Hr.Application.EducationLeaveApplications.SearchEducationLeaveApplications;

public class SearchEducationLeaveApplicationsResponse(
    List<SearchEducationLeaveApplicationsResponseItem> data,
    int totalCount,
    int page,
    int pageSize)
    : PagedListResponse<SearchEducationLeaveApplicationsResponseItem>(data, totalCount, page, pageSize);

public class SearchEducationLeaveApplicationsResponseItem
{
    public Guid Id { get; set; }
    public int ApplicationNumber { get; set; }
    public string Status { get; set; } = null!;
    public ResponseObjectWithName Employee { get; set; } = null!;
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public ResponseObjectWithName CreatedBy { get; set; } = null!;
    public ResponseObjectWithName? Order { get; init; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
