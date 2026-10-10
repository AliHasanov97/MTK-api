using MTK.Common.Presentation.Responses;
using MTK.Common.Domain.Queries;
using MTK.Modules.Hr.Domain.EducationalInstitutions;

namespace MTK.Modules.Hr.Application.EducationalInstitutions.SearchEducationalInstitutions;

public class SearchEducationalInstitutionsResponse(
    List<SearchEducationalInstitutionsResponseItem> data,
    int totalCount,
    int page,
    int pageSize)
    : PagedListResponse<SearchEducationalInstitutionsResponseItem>(data, totalCount, page, pageSize);

public class SearchEducationalInstitutionsResponseItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
