using MTK.Modules.Hr.Domain.Employees;

namespace MTK.Modules.Hr.Application.EmployeeEducationHistories.AddEmployeeEducationHistory;

public class AddEmployeeEducationHistoryResponse
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
