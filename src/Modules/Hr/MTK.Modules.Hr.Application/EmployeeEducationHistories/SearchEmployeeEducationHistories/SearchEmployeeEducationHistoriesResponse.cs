using MTK.Common.Presentation.Responses;
using MTK.Common.Domain.Queries;
using MTK.Modules.Hr.Domain.Employees;

namespace MTK.Modules.Hr.Application.EmployeeEducationHistories.SearchEmployeeEducationHistories;

public class SearchEmployeeEducationHistoriesResponse(
    List<SearchEmployeeEducationHistoriesResponseItem> data,
    int totalCount,
    int page,
    int pageSize)
    : PagedListResponse<SearchEmployeeEducationHistoriesResponseItem>(data, totalCount, page, pageSize);

public class SearchEmployeeEducationHistoriesResponseItem
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid EducationalInstitutionId { get; set; }
    public string? EducationalInstitutionName { get; set; }
    public EducationLevel EducationLevel { get; set; }
    public string Faculty { get; set; } = string.Empty;
    public string? Specialty { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public string? DiplomaNumber { get; set; }
    public string? RegisterNumber { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
