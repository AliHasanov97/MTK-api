using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.EducationalInstitutions;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Users;

namespace MTK.Modules.Hr.Domain.EmployeeEducationHistories;

public sealed class EmployeeEducationHistory : Entity
{
    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; }

    public Guid EducationalInstitutionId { get; private set; }
    public EducationalInstitution EducationalInstitution { get; private set; } = null!;

    public EducationLevel EducationLevel { get; private set; }
    public string Faculty { get; private set; } = string.Empty;
    public string? Specialty { get; private set; }
    public DateTimeOffset StartDate { get; private set; }
    public DateTimeOffset? EndDate { get; private set; }
    public string? DiplomaNumber { get; private set; }
    public string? RegisterNumber { get; private set; }

    public Guid CreatedById { get; private set; }
    public User CreatedBy { get; private set; } = null!;

    private EmployeeEducationHistory() { }

    public static EmployeeEducationHistory Create(
        Guid employeeId,
        Guid educationalInstitutionId,
        EducationLevel educationLevel,
        string faculty,
        string? specialty,
        DateTimeOffset startDate,
        DateTimeOffset? endDate,
        string? diplomaNumber,
        string? registerNumber,
        Guid createdById)
    {
        var history = new EmployeeEducationHistory
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            EducationalInstitutionId = educationalInstitutionId,
            EducationLevel = educationLevel,
            Faculty = faculty,
            Specialty = specialty,
            StartDate = startDate,
            EndDate = endDate,
            DiplomaNumber = diplomaNumber,
            RegisterNumber = registerNumber,
            CreatedById = createdById
        };

        history.RaiseDomainEvent(new EmployeeEducationHistoryChangedDomainEvent { EmployeeId = employeeId });

        return history;
    }

    public void Update(
        Guid educationalInstitutionId,
        EducationLevel educationLevel,
        string faculty,
        string? specialty,
        DateTimeOffset startDate,
        DateTimeOffset? endDate,
        string? diplomaNumber,
        string? registerNumber)
    {
        EducationalInstitutionId = educationalInstitutionId;
        EducationLevel = educationLevel;
        Faculty = faculty;
        Specialty = specialty;
        StartDate = startDate;
        EndDate = endDate;
        DiplomaNumber = diplomaNumber;
        RegisterNumber = registerNumber;

        RaiseDomainEvent(new EmployeeEducationHistoryChangedDomainEvent { EmployeeId = EmployeeId });
    }
}
