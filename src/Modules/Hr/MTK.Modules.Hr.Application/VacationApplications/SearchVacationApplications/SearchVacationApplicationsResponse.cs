using MTK.Common.Presentation.Responses;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Hr.Application.VacationApplications.SearchVacationApplications;

public sealed class SearchVacationApplicationsResponse(
    List<SearchVacationApplicationsResponseItem> data,
    int totalCount,
    int page,
    int pageSize)
    : PagedListResponse<SearchVacationApplicationsResponseItem>(data, totalCount, page, pageSize);

public sealed class SearchVacationApplicationsResponseItem
{
    public Guid Id { get; set; }
    public int ApplicationNumber { get; set; }
    public string Status { get; set; } = null!;
    public ResponseObjectWithName Employee { get; set; } = null!;
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public ResponseObjectWithName? Order { get; init; }
    public int TotalRequestedDays { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}