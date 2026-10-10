using MTK.Common.Presentation.Responses;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Hr.Application.NoticesOfChangeInWorkingConditions.SearchNoticesOfChangeInWorkingConditions;

public sealed class SearchNoticesOfChangeInWorkingConditionsResponse(
    List<SearchNoticesOfChangeInWorkingConditionsResponseItem> data,
    int totalCount,
    int page,
    int pageSize)
    : PagedListResponse<SearchNoticesOfChangeInWorkingConditionsResponseItem>(data, totalCount, page, pageSize);

public sealed class SearchNoticesOfChangeInWorkingConditionsResponseItem
{
    public Guid Id { get; init; }
    public int Index { get; init; }
    public ResponseObjectWithName? Employee { get; init; }
    public DateTimeOffset StartDate { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}