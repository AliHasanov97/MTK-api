using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Domain.Employees;
using System.Text.Json.Serialization;

namespace MTK.Modules.Hr.Application.EmployeeEducationHistories.UpdateEmployeeEducationHistory;

public sealed class UpdateEmployeeEducationHistoryCommand : ICommand<UpdateEmployeeEducationHistoryResponse>
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public Guid EducationalInstitutionId { get; set; }
    public EducationLevel EducationLevel { get; set; }
    public string Faculty { get; set; } = string.Empty;
    public string? Specialty { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public string? DiplomaNumber { get; set; }
    public string? RegisterNumber { get; set; }
}
