using MTK.Common.Presentation.Responses;
using MTK.Common.Domain.Queries;
using MTK.Modules.Hr.Domain.Warnings;

namespace MTK.Modules.Hr.Application.Warnings.SearchWarnings;

public sealed class SearchWarningsResponse(
    List<SearchWarningsResponseItem> data,
    int totalCount,
    int page,
    int pageSize)
    : PagedListResponse<SearchWarningsResponseItem>(data, totalCount, page, pageSize);

public sealed class SearchWarningsResponseItem
{
    public Guid Id { get; init; }
    public int OrderNumber { get; init; }
    public DisciplinaryType DisciplinaryType { get; init; }
    public ResponseObjectWithName? Employee { get; init; }
    public ResponseObjectWithName? OrderExecutionSupervisor { get; init; }
    public DateTimeOffset SetDate { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}
