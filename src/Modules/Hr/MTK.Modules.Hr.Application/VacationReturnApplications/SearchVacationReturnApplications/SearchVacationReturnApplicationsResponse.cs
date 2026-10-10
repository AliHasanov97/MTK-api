using MTK.Common.Presentation.Responses;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Hr.Application.VacationReturnApplications.SearchVacationReturnApplications;

public class SearchVacationReturnApplicationsResponse(
    List<SearchVacationReturnApplicationsResponseItem> data,
    int totalCount,
    int page,
    int pageSize)
    : PagedListResponse<SearchVacationReturnApplicationsResponseItem>(data, totalCount, page, pageSize);

public class SearchVacationReturnApplicationsResponseItem
{
    public Guid Id { get; set; }
    public int ApplicationNumber { get; set; }
    public string Status { get; set; } = null!;
    public ResponseObjectWithName Employee { get; set; } = null!;
    public DateTimeOffset ReturnDate { get; set; }
    public ResponseObjectWithName CreatedBy { get; set; } = null!;
    public ResponseObjectWithName? Order { get; init; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}