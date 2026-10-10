using MTK.Common.Domain.Abstractions;


using MTK.Modules.Hr.Domain.EmployeeEducationHistories;
using MTK.Modules.Hr.Domain.EmployeeWorkHistories;
using MTK.Modules.Hr.Domain.EmployeeWorkSchedules;
using MTK.Modules.Hr.Domain.Employees.Events;
using MTK.Modules.Hr.Domain.EmploymentOrders;
using MTK.Modules.Hr.Domain.FileAttachments;
using MTK.Modules.Hr.Domain.JobApplications;
using MTK.Modules.Hr.Domain.Jobs;
using MTK.Modules.Hr.Domain.Users;

namespace MTK.Modules.Hr.Domain.Employees;

public sealed class Employee : SearchableEntity
{
    public int RegisterNumber { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Surname { get; private set; } = string.Empty;
    public string FathersName { get; private set; } = string.Empty;
    public string? Nationality { get; private set; }
    public Gender Gender { get; private set; }
    public DateTimeOffset? BirthDate { get; private set; }
    public string? FinCode { get; private set; }
    public string? IdCardNumber { get; private set; }
    public string? SocialSecurityNumber { get; private set; }
    public string? ContractNumber { get; private set; }
    public string? SalaryBankName { get; private set; }
    public string? EmployeeBankAccountNumber { get; private set; }
    public MaritalStatus? MaritalStatus { get; private set; }
    public int? NumberOfChildren { get; private set; }
    public MilitaryService? MilitaryService { get; private set; }
    public bool Veteran { get; private set; }
    public bool Disability { get; private set; }
    public EducationLevel? Education { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? HomePhoneNumber { get; private set; }
    public string? Email { get; private set; }
    public string? RegisteredAddress { get; private set; }
    public string? CurrentAddress { get; private set; }
    public WorkingDays? WorkingDays { get; private set; }
    public int VacationDays { get; private set; }
    public int? ChildrenUnder14Count { get; private set; }
    public bool IsKarabakhWorker { get; private set; }
    public bool IsSingleParent { get; private set; }
    public bool HasDisabledChild { get; private set; }
    public DateTimeOffset StartWorkDate { get; private set; }
    public Guid JobId { get; private set; }
    public Job Job { get; private set; } = null!;
    public EmployeeStatus IsActive { get; private set; }
    public EmploymentType EmploymentType { get; private set; }
    public Guid? EmploymentOrderId { get; private set; }
    public EmploymentOrder? EmploymentOrder { get; private set; }
    public Guid CreatedById { get; private set; }
    public User CreatedBy { get; private set; } = null!;

    public ICollection<FileAttachment> FileAttachments { get; private set; } = new List<FileAttachment>();
    public ICollection<EmployeeWorkHistory> WorkHistories { get; private set; } = new List<EmployeeWorkHistory>();
    public ICollection<EmployeeEducationHistory> EducationHistories { get; private set; } = new List<EmployeeEducationHistory>();
    public ICollection<EmployeeWorkSchedule> WorkSchedules { get; private set; } = new List<EmployeeWorkSchedule>();

    /// <summary>
    /// Verilən tarix üçün aktiv olan iş qrafikini qaytarır
    /// </summary>
    public EmployeeWorkSchedule? GetScheduleForDate(DateOnly date)
    {
        return WorkSchedules
            .Where(s => s.EffectiveFrom <= date)
            .OrderByDescending(s => s.EffectiveFrom)
            .FirstOrDefault();
    }

    /// <summary>
    /// Cari (ən son) iş qrafikini qaytarır
    /// </summary>
    public EmployeeWorkSchedule? CurrentWorkSchedule =>
        WorkSchedules
            .OrderByDescending(s => s.EffectiveFrom)
            .FirstOrDefault();

    
    private Employee() { }

    public static Employee Create(
        int registerNumber,
        string name,
        string surname,
        string fathersName,
        Gender gender,
        DateTimeOffset startWorkDate,
        Guid jobId,
        Guid createdById,
        Guid? employmentOrderId = null,
        // Personal Info
        string? nationality = null,
        DateTimeOffset? birthDate = null,
        string? finCode = null,
        string? idCardNumber = null,
        string? socialSecurityNumber = null,
        string? contractNumber = null,
        // Bank Info
        string? salaryBankName = null,
        string? employeeBankAccountNumber = null,
        // Family Info
        MaritalStatus? maritalStatus = null,
        int? numberOfChildren = null,
        int? childrenUnder14Count = null,
        // Military & Status
        MilitaryService? militaryService = null,
        bool? veteran = null,
        bool? disability = null,
        bool? isKarabakhWorker = null,
        bool? isSingleParent = null,
        bool? hasDisabledChild = null,
        // Education
        EducationLevel? education = null,
        // Contact Info
        string? phoneNumber = null,
        string? homePhoneNumber = null,
        string? email = null,
        string? registeredAddress = null,
        string? currentAddress = null,
        // Work Info
        WorkingDays? workingDays = null,
        int? vacationDays = null,
        EmploymentType? employmentType = null)
    {
        var employeeId = Guid.NewGuid();

        var employee = new Employee
        {
            Id = employeeId,
            RegisterNumber = registerNumber,
            Name = name,
            Surname = surname,
            FathersName = fathersName,
            Gender = gender,
            StartWorkDate = startWorkDate,
            JobId = jobId,
            IsActive = EmployeeStatus.Active,
            EmploymentType = employmentType ?? EmploymentType.FullTime,
            EmploymentOrderId = employmentOrderId,
            CreatedById = createdById,
            // Personal Info
            Nationality = nationality,
            BirthDate = birthDate,
            FinCode = finCode,
            IdCardNumber = idCardNumber,
            SocialSecurityNumber = socialSecurityNumber,
            ContractNumber = contractNumber,
            // Bank Info
            SalaryBankName = salaryBankName,
            EmployeeBankAccountNumber = employeeBankAccountNumber,
            // Family Info
            MaritalStatus = maritalStatus,
            NumberOfChildren = numberOfChildren,
            ChildrenUnder14Count = childrenUnder14Count,
            // Military & Status
            MilitaryService = militaryService,
            Veteran = veteran ?? false,
            Disability = disability ?? false,
            IsKarabakhWorker = isKarabakhWorker ?? false,
            IsSingleParent = isSingleParent ?? false,
            HasDisabledChild = hasDisabledChild ?? false,
            // Education
            Education = education,
            // Contact Info
            PhoneNumber = phoneNumber,
            HomePhoneNumber = homePhoneNumber,
            Email = email,
            RegisteredAddress = registeredAddress,
            CurrentAddress = currentAddress,
            // Work Info
            WorkingDays = workingDays,
            VacationDays = vacationDays ?? 21 // Default 21 gün (Əmək Məcəlləsi maddə 113)
        };

        // Raise domain event
        employee.RaiseDomainEvent(new EmployeeCreatedDomainEvent
        {
            EmployeeId = employeeId,
            StartWorkDate = startWorkDate
        });

        return employee;
    }

    public void Update(
        string name,
        string surname,
        string fathersName,
        string? nationality,
        DateTimeOffset? birthDate,
        string? finCode,
        string? idCardNumber,
        string? socialSecurityNumber,
        string? contractNumber,
        string? salaryBankName,
        string? employeeBankAccountNumber,
        MaritalStatus? maritalStatus,
        int? numberOfChildren,
        int? childrenUnder14Count,
        MilitaryService? militaryService,
        bool? veteran,
        bool? disability,
        bool? isKarabakhWorker,
        bool? isSingleParent,
        bool? hasDisabledChild,
        EducationLevel? education,
        string? phoneNumber,
        string? homePhoneNumber,
        string? email,
        string? registeredAddress,
        string? currentAddress,
        WorkingDays? workingDays,
        int? vacationDays,
        EmployeeStatus? isActive,
        EmploymentType? employmentType,
        Gender? gender = null)
    {
        if (gender.HasValue) Gender = gender.Value;
        if (name is not null) Name = string.IsNullOrWhiteSpace(name) ? null : name;
        if (surname is not null) Surname = string.IsNullOrWhiteSpace(surname) ? null : surname;
        if (fathersName is not null) FathersName = string.IsNullOrWhiteSpace(fathersName) ? null : fathersName;

        if (nationality is not null) Nationality = string.IsNullOrWhiteSpace(nationality) ? null : nationality;
        if (birthDate.HasValue) BirthDate = birthDate.Value;
        if (finCode is not null) FinCode = string.IsNullOrWhiteSpace(finCode) ? null : finCode;
        if (idCardNumber is not null) IdCardNumber = string.IsNullOrWhiteSpace(idCardNumber) ? null : idCardNumber;
        if (socialSecurityNumber is not null) SocialSecurityNumber = string.IsNullOrWhiteSpace(socialSecurityNumber) ? null : socialSecurityNumber;
        if (contractNumber is not null) ContractNumber = string.IsNullOrWhiteSpace(contractNumber) ? null : contractNumber;
        if (salaryBankName is not null) SalaryBankName = string.IsNullOrWhiteSpace(salaryBankName) ? null : salaryBankName;
        if (employeeBankAccountNumber is not null) EmployeeBankAccountNumber = string.IsNullOrWhiteSpace(employeeBankAccountNumber) ? null : employeeBankAccountNumber;
        if (maritalStatus.HasValue) MaritalStatus = maritalStatus.Value;
        if (numberOfChildren.HasValue) NumberOfChildren = numberOfChildren.Value;
        if (childrenUnder14Count.HasValue) ChildrenUnder14Count = childrenUnder14Count.Value;
        if (militaryService.HasValue) MilitaryService = militaryService.Value;
        if (veteran.HasValue) Veteran = veteran.Value;
        if (disability.HasValue) Disability = disability.Value;
        if (isKarabakhWorker.HasValue) IsKarabakhWorker = isKarabakhWorker.Value;
        if (isSingleParent.HasValue) IsSingleParent = isSingleParent.Value;
        if (hasDisabledChild.HasValue) HasDisabledChild = hasDisabledChild.Value;
        if (education.HasValue) Education = education.Value;
        if (phoneNumber is not null) PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber;
        if (homePhoneNumber is not null) HomePhoneNumber = string.IsNullOrWhiteSpace(homePhoneNumber) ? null : homePhoneNumber;
        if (email is not null) Email = string.IsNullOrWhiteSpace(email) ? null : email;
        if (registeredAddress is not null) RegisteredAddress = string.IsNullOrWhiteSpace(registeredAddress) ? null : registeredAddress;
        if (currentAddress is not null) CurrentAddress = string.IsNullOrWhiteSpace(currentAddress) ? null : currentAddress;
        if (workingDays.HasValue) WorkingDays = workingDays.Value;
        if (vacationDays.HasValue) VacationDays = vacationDays.Value;
        if (isActive.HasValue) IsActive = isActive.Value;
        if (employmentType.HasValue) EmploymentType = employmentType.Value;

        // Raise domain event
        RaiseDomainEvent(new EmployeeUpdatedDomainEvent
        {
            EmployeeId = Id
        });
    }

    public int CalculateTotalWorkExperienceInDays(DateTimeOffset? asOfDate = null)
    {
        var referenceDate = asOfDate ?? DateTimeOffset.UtcNow;

        // Bütün iş müddətlərini bir list-də toplayırıq
        var allPeriods = new List<(DateTimeOffset Start, DateTimeOffset End)>();

        // Keçmiş işlər (WorkHistories)
        foreach (var history in WorkHistories)
        {
            var endDate = history.EndDate ?? referenceDate;

            // Əgər iş referenceDate-dən əvvəl başlayıbsa
            if (history.StartDate <= referenceDate)
            {
                // End date-i referenceDate-dən sonradırsa, referenceDate-ə qədər hesabla
                var effectiveEndDate = endDate > referenceDate ? referenceDate : endDate;
                allPeriods.Add((history.StartDate, effectiveEndDate));
            }
        }

        // Cari şirkətdəki təcrübə (əgər işə başlama tarixi referenceDate-dən əvvəldirsə)
        if (StartWorkDate <= referenceDate)
        {
            allPeriods.Add((StartWorkDate, referenceDate));
        }

        // Üst-üstə düşən müddətləri birləşdiririk
        var mergedPeriods = MergeOverlappingPeriods(allPeriods);

        // Birləşdirilmiş müddətlərin cəmini götürürük
        var totalDays = 0;
        foreach (var period in mergedPeriods)
        {
            totalDays += (int)(period.End - period.Start).TotalDays;
        }

        return totalDays;
    }

    private static List<(DateTimeOffset Start, DateTimeOffset End)> MergeOverlappingPeriods(
        List<(DateTimeOffset Start, DateTimeOffset End)> periods)
    {
        if (periods.Count == 0)
            return new List<(DateTimeOffset Start, DateTimeOffset End)>();

        // Başlanğıc tarixinə görə sıralayırıq
        var sortedPeriods = periods.OrderBy(p => p.Start).ToList();

        var merged = new List<(DateTimeOffset Start, DateTimeOffset End)>();
        var current = sortedPeriods[0];

        for (int i = 1; i < sortedPeriods.Count; i++)
        {
            var next = sortedPeriods[i];

            // Üst-üstə düşürsə və ya bitişiksə birləşdiririk
            if (next.Start <= current.End)
            {
                // Birləşdir: end date-i ən böyüyünü götür
                current = (current.Start, next.End > current.End ? next.End : current.End);
            }
            else
            {
                // Üst-üstə düşmür, current-i əlavə edib next-ə keç
                merged.Add(current);
                current = next;
            }
        }

        // Sonuncu müddəti əlavə et
        merged.Add(current);

        return merged;
    }

    public WorkExperienceBreakdown CalculateTotalWorkExperience(DateTimeOffset? asOfDate = null)
    {
        var totalDays = CalculateTotalWorkExperienceInDays(asOfDate);
        return WorkExperienceBreakdown.FromDays(totalDays);
    }

    public WorkExperienceBreakdown CalculateOrganizationWorkExperience()
    {
        return WorkExperienceBreakdown.FromDateRange(StartWorkDate);
    }

    public void UpdatePosition(Guid newJobId)
    {
        JobId = newJobId;
    }

    public void UpdateEmploymentType(EmploymentType newEmploymentType)
    {
        EmploymentType = newEmploymentType;
    }
}
