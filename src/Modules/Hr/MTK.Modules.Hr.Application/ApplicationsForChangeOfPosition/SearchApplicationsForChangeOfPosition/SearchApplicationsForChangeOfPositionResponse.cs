using MTK.Common.Presentation.Responses;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Hr.Application.ApplicationsForChangeOfPosition.SearchApplicationsForChangeOfPosition;

public sealed class SearchApplicationsForChangeOfPositionResponse(
    List<SearchApplicationsForChangeOfPositionResponseItem> data,
    int totalCount,
    int page,
    int pageSize)
    : PagedListResponse<SearchApplicationsForChangeOfPositionResponseItem>(data, totalCount, page, pageSize);

public sealed class SearchApplicationsForChangeOfPositionResponseItem
{
    public Guid Id { get; init; }
    public int ApplicationNumber { get; init; }
    public string Status { get; init; } = null!;
    public ResponseObjectWithName? Employee { get; init; }
    public ResponseObjectWithName? CurrentJob { get; init; }
    public ResponseObjectWithName? NewJob { get; init; }
    public ResponseObjectWithName? Order { get; init; }
    public DateTimeOffset SetDate { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}
