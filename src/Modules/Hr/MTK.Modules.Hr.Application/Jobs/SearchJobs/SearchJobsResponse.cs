using MTK.Common.Presentation.Responses;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Hr.Application.Jobs.SearchJobs;

public class SearchJobsResponse(
    List<SearchJobsResponseItem> data,
    int totalCount,
    int page,
    int pageSize)
    : PagedListResponse<SearchJobsResponseItem>(data, totalCount, page, pageSize);

public class SearchJobsResponseItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
