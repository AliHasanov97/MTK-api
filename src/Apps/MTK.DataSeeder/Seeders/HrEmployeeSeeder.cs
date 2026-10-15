using Bogus;
using MediatR;
using Microsoft.Extensions.Logging;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.EmployeeEducationHistories.AddEmployeeEducationHistory;
using MTK.Modules.Hr.Application.EmployeeWorkHistories.AddEmployeeWorkHistory;
using MTK.Modules.Hr.Application.Employees.AddEmployee;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.JobApplications;
using MTK.Modules.Hr.Domain.Users;

namespace MTK.DataSeeder.Seeders;

/// <summary>
/// İşçiləri (Employees), onların iş/təhsil tarixçəsini seed edir. Qəsdən məzuniyyət (və digər)
/// ərizə/əmrlərinə TOXUNMUR — yalnız HR-ın özək/master məlumatı (bax: HrJobSeeder,
/// HrEducationalInstitutionSeeder) və işçi profilləri doldurulur. Əvvəlcə HrJobSeeder və
/// HrEducationalInstitutionSeeder-in işləmiş olması lazımdır (Program.cs-də sıra ilə çağırılır).
/// </summary>
public sealed class HrEmployeeSeeder
{
    private readonly ISender _sender;
    private readonly ILogger<HrEmployeeSeeder> _logger;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    // CreatedBy FK-nın işarə etdiyi sistem istifadəçisi — FakeUserContext.UserId ilə eynidir.
    private static readonly Guid SystemUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    private static readonly string[] CompanyNames =
    {
        "Günəş İnşaat MMC", "Bakı Tikinti Holdinq", "Araz Servis MMC", "Zirvə Group",
        "Xəzər Tikinti", "Ekobina MMC", "Nurlan Servis", "Şərq Tikinti MMC",
    };

    private static readonly string[] BankNames = { "Kapital Bank", "PAŞA Bank", "ABB", "Bank of Baku", "Rabitəbank" };

    public HrEmployeeSeeder(
        ISender sender,
        ILogger<HrEmployeeSeeder> logger,
        IEmployeeRepository employeeRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _sender = sender;
        _logger = logger;
        _employeeRepository = employeeRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    private async Task EnsureSystemUserAsync()
    {
        var existing = await _userRepository.GetByIdDefaultAsync(SystemUserId);
        if (existing is not null)
        {
            return;
        }

        var systemUser = User.Create(SystemUserId, "Sistem", "İstifadəçi", "system@mtk.local", null, "system");
        _userRepository.Add(systemUser);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task SeedAsync()
    {
        var existingEmployees = await _employeeRepository.ListAsync();
        if (existingEmployees.Any())
        {
            _logger.LogInformation("  ℹ️  Bazada artıq {Count} işçi mövcuddur", existingEmployees.Count);
            return;
        }

        var jobIds = new List<Guid>(HrJobSeeder.JobIds);
        var institutionIds = new List<Guid>(HrEducationalInstitutionSeeder.InstitutionIds);

        if (jobIds.Count == 0)
        {
            _logger.LogError("  ❌ Vəzifə (Job) yoxdur — əvvəlcə HR vəzifələrini seed edin");
            return;
        }

        await EnsureSystemUserAsync();

        var faker = new Faker("tr"); // Bogus az_AZ dəstəkləmir, "tr" ən yaxın adlar üçün istifadə olunur
        const int employeeCount = 30;
        var now = DateTimeOffset.UtcNow;
        var created = 0;

        for (var i = 0; i < employeeCount; i++)
        {
            var gender = faker.Random.Bool() ? Gender.Male : Gender.Female;
            var firstName = gender == Gender.Male ? faker.Name.FirstName(Bogus.DataSets.Name.Gender.Male) : faker.Name.FirstName(Bogus.DataSets.Name.Gender.Female);
            var surname = faker.Name.LastName();
            var fathersName = faker.Name.FirstName(Bogus.DataSets.Name.Gender.Male);

            // Təcrübə illərini qəsdən geniş yaysaq, Maddə 115 staj bonusunun (5/10/15 il) bütün
            // pillələri sonradan manual olaraq (məzuniyyət balansı vasitəsilə) sınanabilsin.
            var yearsAtCompany = faker.Random.Int(0, 18);
            var startWorkDate = now.AddYears(-yearsAtCompany).AddDays(-faker.Random.Int(0, 300));
            var ageAtHire = faker.Random.Int(22, 55);
            var birthDate = startWorkDate.AddYears(-ageAtHire).AddDays(-faker.Random.Int(0, 300));

            var numberOfChildren = faker.Random.Int(0, 4);
            var childrenUnder14 = numberOfChildren == 0 ? 0 : faker.Random.Int(0, Math.Min(numberOfChildren, 3));

            var command = new AddEmployeeCommand
            {
                Name = firstName,
                Surname = surname,
                FathersName = fathersName,
                Gender = gender,
                StartWorkDate = startWorkDate,
                JobId = faker.PickRandom(jobIds),

                Nationality = faker.Random.Bool(0.95f) ? "Azərbaycan" : "Rusiya",
                BirthDate = birthDate,
                FinCode = faker.Random.AlphaNumeric(7).ToUpperInvariant(),
                IdCardNumber = $"AZE{faker.Random.Number(10000000, 99999999)}",
                SocialSecurityNumber = faker.Random.ReplaceNumbers("###-###-###"),
                ContractNumber = $"C-{(i + 1):D4}",

                SalaryBankName = faker.PickRandom(BankNames),
                EmployeeBankAccountNumber = $"AZ{faker.Random.Number(10, 99)}NABZ{faker.Random.Number(10000000, 99999999)}",
                BaseSalary = faker.Random.Int(500, 3000),

                MaritalStatus = faker.Random.Bool() ? MaritalStatus.Married : MaritalStatus.Single,
                NumberOfChildren = numberOfChildren,
                ChildrenUnder14Count = childrenUnder14,

                MilitaryService = gender == Gender.Male
                    ? faker.PickRandom(new[] { MilitaryService.NotServed, MilitaryService.Served, MilitaryService.Exempt })
                    : null,
                Veteran = faker.Random.Bool(0.05f),
                Disability = faker.Random.Bool(0.05f),
                IsKarabakhWorker = faker.Random.Bool(0.08f),
                IsSingleParent = faker.Random.Bool(0.06f),
                HasDisabledChild = faker.Random.Bool(0.05f),

                Education = faker.PickRandom(new[]
                {
                    EducationLevel.HigherBachelor, EducationLevel.HigherMaster, EducationLevel.SecondarySpecialized,
                    EducationLevel.GeneralComplete,
                }),

                PhoneNumber = faker.Phone.PhoneNumber("+994##-###-##-##"),
                HomePhoneNumber = faker.Random.Bool(0.4f) ? faker.Phone.PhoneNumber("+994-12-###-##-##") : null,
                Email = faker.Internet.Email(firstName, surname).ToLowerInvariant(),
                RegisteredAddress = $"Bakı şəhəri, {faker.Address.StreetName()} küç. {faker.Random.Number(1, 200)}",
                CurrentAddress = faker.Random.Bool(0.7f) ? null : $"Bakı şəhəri, {faker.Address.StreetName()} küç. {faker.Random.Number(1, 200)}",

                WorkingDays = faker.Random.Bool(0.8f) ? WorkingDays.FiveDays : WorkingDays.SixDays,
                VacationDays = 21,
                EmploymentType = faker.Random.Bool(0.9f) ? EmploymentType.FullTime : EmploymentType.HalfTime,
            };

            var result = await _sender.Send(command);
            if (!result.IsSuccess)
            {
                _logger.LogError("  ❌ İşçi yaradıla bilmədi ({Name} {Surname}): {Error}", firstName, surname, result.Error);
                continue;
            }

            created++;
            var employeeId = result.Value.Id;

            // ~60% işçi üçün bu təşkilatdan əvvəlki 1-2 iş yeri (ümumi staj hesablamasına daxil olur).
            if (faker.Random.Bool(0.6f))
            {
                var priorJobsCount = faker.Random.Int(1, 2);
                var cursor = startWorkDate.AddDays(-faker.Random.Int(30, 180));

                for (var j = 0; j < priorJobsCount; j++)
                {
                    var durationYears = faker.Random.Int(1, 5);
                    var historyEnd = cursor;
                    var historyStart = historyEnd.AddYears(-durationYears);

                    await _sender.Send(new AddEmployeeWorkHistoryCommand
                    {
                        EmployeeId = employeeId,
                        CompanyName = faker.PickRandom(CompanyNames),
                        Position = faker.Name.JobTitle(),
                        StartDate = historyStart,
                        EndDate = historyEnd,
                        Notes = null,
                    });

                    cursor = historyStart.AddDays(-faker.Random.Int(0, 90));
                }
            }

            // ~70% işçi üçün bir təhsil qeydi (əgər təhsil ocağı seed edilibsə).
            if (institutionIds.Count > 0 && faker.Random.Bool(0.7f))
            {
                var eduEnd = startWorkDate.AddDays(-faker.Random.Int(30, 365));
                var eduStart = eduEnd.AddYears(-faker.Random.Int(4, 6));

                await _sender.Send(new AddEmployeeEducationHistoryCommand
                {
                    EmployeeId = employeeId,
                    EducationalInstitutionId = faker.PickRandom(institutionIds),
                    EducationLevel = command.Education ?? EducationLevel.HigherBachelor,
                    Faculty = faker.Name.JobTitle(),
                    Specialty = null,
                    StartDate = eduStart,
                    EndDate = eduEnd,
                    DiplomaNumber = faker.Random.AlphaNumeric(8).ToUpperInvariant(),
                    RegisterNumber = faker.Random.Number(1000, 9999).ToString(),
                });
            }

            if (created % 10 == 0)
            {
                _logger.LogInformation("  ✅ {Count} işçi yaradıldı...", created);
            }
        }

        _logger.LogInformation("  📊 Cəmi {Count} işçi yaradıldı", created);
    }
}
