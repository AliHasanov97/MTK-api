using MTK.Common.Presentation.Responses;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Hr.Application.LaborCodeCases.SearchLaborCodeCases;

public class SearchLaborCodeCasesResponse(
    List<SearchLaborCodeCasesResponseItem> data,
    int totalCount,
    int page,
    int pageSize)
    : PagedListResponse<SearchLaborCodeCasesResponseItem>(data, totalCount, page, pageSize);

public class SearchLaborCodeCasesResponseItem
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public string? ParentName { get; set; }
    public bool IsActive { get; set; }
}
