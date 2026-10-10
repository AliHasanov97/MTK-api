using MTK.Common.Presentation.Responses;
using MTK.Common.Domain.Queries;
using MTK.Modules.Hr.Domain.Applications;

namespace MTK.Modules.Hr.Application.Applications.SearchApplications;

public sealed class SearchApplicationsResponse(
    List<SearchApplicationsResponseItem> data,
    int totalCount,
    int page,
    int pageSize)
    : PagedListResponse<SearchApplicationsResponseItem>(data, totalCount, page, pageSize);

public sealed class SearchApplicationsResponseItem
{
    public Guid Id { get; init; }
    public int ApplicationNumber { get; init; }
    public ApplicationType Type { get; init; }
    public ResponseObjectWithName? Employee { get; init; }
    public ResponseObjectWithName? JobApplicant { get; init; }
    public ResponseObjectWithName? CreatedBy { get; init; }
    public ResponseObjectWithName? Order { get; init; }

    public string Status { get; init; }

    public DateTimeOffset CreatedAt { get; init; }
}