using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.JobApplications;

namespace MTK.Modules.Hr.Application.Employees.AddEmployee;

public class AddEmployeeResponse
{
    public Guid Id { get; set; }
    public int RegisterNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string FathersName { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public string? PhoneNumber { get; set; }
    public string? HomePhoneNumber { get; set; }
    public string? CurrentAddress { get; set; }
    public DateTimeOffset StartWorkDate { get; set; }
    public ResponseObjectWithName? Job { get; set; }
    public EmployeeStatus IsActive { get; set; }
    public EmploymentType EmploymentType { get; set; }
    public ResponseObjectWithName CreatedBy { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
}
