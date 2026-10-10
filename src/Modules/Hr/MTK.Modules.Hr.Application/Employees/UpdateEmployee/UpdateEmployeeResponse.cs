using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.JobApplications;
using WorkExperienceBreakdown = MTK.Modules.Hr.Domain.Employees.WorkExperienceBreakdown;

namespace MTK.Modules.Hr.Application.Employees.UpdateEmployee;

public class UpdateEmployeeResponse
{
    public Guid Id { get; set; }
    public int RegisterNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string FathersName { get; set; } = string.Empty;
    public string? Nationality { get; set; }
    public Gender Gender { get; set; }
    public DateTimeOffset? BirthDate { get; set; }
    public string? FinCode { get; set; }
    public string? IdCardNumber { get; set; }
    public string? SocialSecurityNumber { get; set; }
    public string? ContractNumber { get; set; }
    public string? SalaryBankName { get; set; }
    public string? EmployeeBankAccountNumber { get; set; }
    public MaritalStatus? MaritalStatus { get; set; }
    public int? NumberOfChildren { get; set; }
    public int? ChildrenUnder14Count { get; set; }
    public MilitaryService? MilitaryService { get; set; }
    public bool Veteran { get; set; }
    public bool Disability { get; set; }
    public bool IsKarabakhWorker { get; set; }
    public bool IsSingleParent { get; set; }
    public bool HasDisabledChild { get; set; }
    public EducationLevel? Education { get; set; }
    public string? PhoneNumber { get; set; }
    public string? HomePhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? RegisteredAddress { get; set; }
    public string? CurrentAddress { get; set; }
    public WorkingDays? WorkingDays { get; set; }
    public int VacationDays { get; set; }
    public WorkExperienceBreakdown TotalWorkExperience { get; set; } = null!;
    public WorkExperienceBreakdown OrganizationWorkExperience { get; set; } = null!;
    public DateTimeOffset StartWorkDate { get; set; }
    public ResponseObjectWithName? Job { get; set; }
    public EmployeeStatus IsActive { get; set; }
    public EmploymentType EmploymentType { get; set; }
    public Guid? EmploymentOrderId { get; set; }
    public ResponseObjectWithName CreatedBy { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
