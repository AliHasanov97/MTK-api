using MTK.Common.Presentation.Responses;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Hr.Application.UnexcusedAbsences.SearchUnexcusedAbsences;

public sealed class SearchUnexcusedAbsencesResponse(
    List<SearchUnexcusedAbsencesResponseItem> data,
    int totalCount,
    int page,
    int pageSize)
    : PagedListResponse<SearchUnexcusedAbsencesResponseItem>(data, totalCount, page, pageSize);

public sealed class SearchUnexcusedAbsencesResponseItem
{
    public Guid Id { get; init; }
    public int OrderNumber { get; init; }
    public ResponseObjectWithName? Employee { get; init; }
    public DateTimeOffset SetDate { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}
