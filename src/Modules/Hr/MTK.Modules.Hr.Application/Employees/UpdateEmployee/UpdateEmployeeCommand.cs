using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.JobApplications;
using System.Text.Json.Serialization;

namespace MTK.Modules.Hr.Application.Employees.UpdateEmployee;

public sealed class UpdateEmployeeCommand : ICommand<UpdateEmployeeResponse>
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public Gender? Gender { get; set; }
    public string? Surname { get; set; }
    public string? FathersName { get; set; }
    public string? Nationality { get; set; }
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
    public bool? Veteran { get; set; }
    public bool? Disability { get; set; }
    public bool? IsKarabakhWorker { get; set; }
    public bool? IsSingleParent { get; set; }
    public bool? HasDisabledChild { get; set; }
    public EducationLevel? Education { get; set; }
    public string? PhoneNumber { get; set; }
    public string? HomePhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? RegisteredAddress { get; set; }
    public string? CurrentAddress { get; set; }
    public WorkingDays? WorkingDays { get; set; }
    public int? VacationDays { get; set; }
    public EmployeeStatus? IsActive { get; set; }
    public EmploymentType? EmploymentType { get; set; }
}
