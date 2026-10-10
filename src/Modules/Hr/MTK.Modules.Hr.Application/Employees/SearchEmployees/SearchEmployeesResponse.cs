using MTK.Common.Presentation.Responses;
using MTK.Common.Domain.Queries;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.JobApplications;

namespace MTK.Modules.Hr.Application.Employees.SearchEmployees;

public class SearchEmployeesResponse(
    List<SearchEmployeesResponseItem> data,
    int totalCount,
    int page,
    int pageSize)
    : PagedListResponse<SearchEmployeesResponseItem>(data, totalCount, page, pageSize);

public class SearchEmployeesResponseItem
{
    public Guid Id { get; set; }
    public int RegisterNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string FathersName { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public DateTimeOffset? BirthDate { get; set; }
    public int? Age { get; set; }
    public string? FinCode { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public EducationLevel? Education { get; set; }
    public MaritalStatus? MaritalStatus { get; set; }
    public DateTimeOffset StartWorkDate { get; set; }
    public ResponseObjectWithName? Job { get; set; }
    public EmployeeStatus IsActive { get; set; }
    public EmploymentType EmploymentType { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
