using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.JobApplications;

namespace MTK.Modules.Hr.Application.Employees.AddEmployee;

public sealed class AddEmployeeCommand : ICommand<AddEmployeeResponse>
{
    // Required Fields
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string FathersName { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public DateTimeOffset StartWorkDate { get; set; }
    public Guid JobId { get; set; }

    // Personal Info
    public string? Nationality { get; set; }
    public DateTimeOffset? BirthDate { get; set; }
    public string? FinCode { get; set; }
    public string? IdCardNumber { get; set; }
    public string? SocialSecurityNumber { get; set; }
    public string? ContractNumber { get; set; }

    // Bank Info
    public string? SalaryBankName { get; set; }
    public string? EmployeeBankAccountNumber { get; set; }

    // Family Info
    public MaritalStatus? MaritalStatus { get; set; }
    public int? NumberOfChildren { get; set; }
    public int? ChildrenUnder14Count { get; set; }

    // Military & Status
    public MilitaryService? MilitaryService { get; set; }
    public bool? Veteran { get; set; }
    public bool? Disability { get; set; }
    public bool? IsKarabakhWorker { get; set; }
    public bool? IsSingleParent { get; set; }
    public bool? HasDisabledChild { get; set; }

    // Education
    public EducationLevel? Education { get; set; }

    // Contact Info
    public string? PhoneNumber { get; set; }
    public string? HomePhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? RegisteredAddress { get; set; }
    public string? CurrentAddress { get; set; }

    // Work Info
    public WorkingDays? WorkingDays { get; set; }
    public int? VacationDays { get; set; }
    public EmploymentType? EmploymentType { get; set; }
}
