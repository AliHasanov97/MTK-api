using MTK.Common.Presentation.Responses;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Hr.Application.UnpaidLeaveApplications.SearchUnpaidLeaveApplications;

public class SearchUnpaidLeaveApplicationsResponse(
    List<SearchUnpaidLeaveApplicationsResponseItem> data,
    int totalCount,
    int page,
    int pageSize)
    : PagedListResponse<SearchUnpaidLeaveApplicationsResponseItem>(data, totalCount, page, pageSize);

public class SearchUnpaidLeaveApplicationsResponseItem
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
