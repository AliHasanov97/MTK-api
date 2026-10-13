using Bogus;
using MediatR;
using Microsoft.Extensions.Logging;
using MTK.Modules.Payments.Application.Contracts.Commands.AddContractService;
using MTK.Modules.Payments.Application.Contracts.Commands.ChangeContractStatus;
using MTK.Modules.Payments.Application.Contracts.Commands.CreateContract;
using MTK.Modules.Payments.Domain.Contracts;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.DataSeeder.Seeders;

/// <summary>
/// Müqavilə (contract) və xidmətlər (services) seed edir
/// </summary>
public sealed class ContractSeeder
{
    private readonly ISender _sender;
    private readonly ILogger<ContractSeeder> _logger;
    private readonly IContractRepository _contractRepository;

    public static readonly List<Guid> ContractIds = new();

    public ContractSeeder(ISender sender, ILogger<ContractSeeder> logger, IContractRepository contractRepository)
    {
        _sender = sender;
        _logger = logger;
        _contractRepository = contractRepository;
    }

    public async Task SeedAsync()
    {
        // Əvvəlcə bütün mövcud müqavilələri bazadan oxu
        var existingContracts = await _contractRepository.ListAsync();
        ContractIds.Clear();
        ContractIds.AddRange(existingContracts.Select(c => c.Id));

        if (ContractIds.Any())
        {
            _logger.LogInformation("  ℹ️  Bazada artıq {Count} müqavilə mövcuddur", ContractIds.Count);
            return;
        }

        if (!VendorSeeder.VendorIds.Any())
        {
            _logger.LogWarning("  ⚠️  Tədarükçülər mövcud deyil. Əvvəlcə VendorSeeder işlətməlisiniz.");
            return;
        }

        var faker = new Faker("tr"); // Türk dili (Bogus az_AZ dəstəkləmir)
        var currentYear = DateTimeOffset.UtcNow.Year;

        // Hər tədarükçü üçün bir müqavilə yaradaq
        var contractCounter = 1;

        foreach (var vendorId in VendorSeeder.VendorIds)
        {
            // Müqavilə nömrəsi: YYYY/XXX formatında
            var contractNumber = $"{currentYear}/{contractCounter:D3}";

            // Müqavilə tarixi: bu ilin əvvəlindən başlayır, 1 il müddətli
            var startDate = new DateTimeOffset(currentYear, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var endDate = new DateTimeOffset(currentYear, 12, 31, 23, 59, 59, TimeSpan.Zero);

            // Müqavilə yarat
            var createContractCommand = new CreateContractCommand(
                contractNumber,
                vendorId,
                startDate,
                endDate,
                null, // CreatedByUserId
                faker.Lorem.Sentence(5)); // Random qeyd

            var contractResult = await _sender.Send(createContractCommand);

            if (contractResult.IsFailure)
            {
                _logger.LogError("  ❌ Müqavilə yaradıla bilmədi: {Error}", contractResult.Error);
                continue;
            }

            var contractId = contractResult.Value;
            ContractIds.Add(contractId);

            // Müqaviləyə xidmətlər əlavə et
            var services = GetServicesForContract(contractCounter);

            foreach (var service in services)
            {
                var addServiceCommand = new AddContractServiceCommand(
                    contractId,
                    service.Name,
                    service.UnitPrice,
                    service.BillingPeriod,
                    service.Description,
                    null, // ServiceStartDate (müqavilənin başlanğıc tarixindən başlayır)
                    null, // ServiceEndDate (müqavilənin bitmə tarixinə qədər)
                    30); // PaymentTermDays (30 gün ödəniş müddəti)

                var serviceResult = await _sender.Send(addServiceCommand);

                if (serviceResult.IsFailure)
                {
                    _logger.LogError("  ❌ Xidmət əlavə edilə bilmədi: {Error}", serviceResult.Error);
                }
            }

            // Müqaviləni aktivləşdir
            var activateCommand = new ActivateContractCommand(contractId);
            var activateResult = await _sender.Send(activateCommand);

            if (activateResult.IsFailure)
            {
                _logger.LogError("  ❌ Müqavilə aktivləşdirilə bilmədi: {Error}", activateResult.Error);
            }
            else
            {
                _logger.LogInformation("  ✅ Müqavilə yaradıldı və aktivləşdirildi: {Number} ({ServiceCount} xidmət)",
                    contractNumber, services.Count);
            }

            contractCounter++;
        }

        _logger.LogInformation("  📊 Cəmi {Count} müqavilə yaradıldı", ContractIds.Count);
    }

    private static List<(string Name, decimal UnitPrice, BillingPeriod BillingPeriod, string? Description)> GetServicesForContract(int contractIndex)
    {
        // Hər müqavilə üçün müxtəlif xidmətlər təyin edirik
        return contractIndex switch
        {
            1 => new List<(string, decimal, BillingPeriod, string?)> // Lift Texniki Xidmət
            {
                ("Liftlərin aylıq texniki xidməti", 150.00m, BillingPeriod.Monthly, "3 liftin texniki xidməti və təmiri"),
                ("Liftlərin rüblük yoxlanması", 300.00m, BillingPeriod.Quarterly, "Dövlət standartlarına uyğun yoxlama"),
            },
            2 => new List<(string, decimal, BillingPeriod, string?)> // Qaz Təchizatı
            {
                ("Qaz təchizatı (aylıq abonent haqqı)", 50.00m, BillingPeriod.Monthly, "Qaz sayğacının saxlanması və oxunması"),
                ("Qaz sisteminin illik texniki yoxlanması", 500.00m, BillingPeriod.Yearly, "Qaz kəmərlərinin və avadanlığın yoxlanması"),
            },
            3 => new List<(string, decimal, BillingPeriod, string?)> // Su Kanalı
            {
                ("Su və kanalizasiya (aylıq)", 80.00m, BillingPeriod.Monthly, "Su təchizatı və çirkab suların axıdılması"),
            },
            4 => new List<(string, decimal, BillingPeriod, string?)> // Elektrik Şəbəkəsi
            {
                ("Elektrik enerjisi (aylıq abonent haqqı)", 100.00m, BillingPeriod.Monthly, "Ümumi sahələrin işıqlandırılması"),
                ("Trafo məntəqəsinin aylıq texniki xidməti", 200.00m, BillingPeriod.Monthly, "Trafonun yoxlanması və təmiri"),
            },
            5 => new List<(string, decimal, BillingPeriod, string?)> // Təmizlik Xidmətləri
            {
                ("Binanın təmizlənməsi (aylıq)", 400.00m, BillingPeriod.Monthly, "Pilləkənlərin, koridorların və ümumi sahələrin təmizlənməsi"),
                ("Həyətin təmizlənməsi (aylıq)", 150.00m, BillingPeriod.Monthly, "Binanın ətrafının təmizlənməsi və zibilin götürülməsi"),
            },
            6 => new List<(string, decimal, BillingPeriod, string?)> // Mühafizə Şirkəti
            {
                ("Mühafizə xidməti (aylıq)", 600.00m, BillingPeriod.Monthly, "24/7 mühafizə xidməti və monitorinq"),
            },
            7 => new List<(string, decimal, BillingPeriod, string?)> // Landşaft Dizayn
            {
                ("Yaşıllığa qulluq (aylıq)", 120.00m, BillingPeriod.Monthly, "Ağac və güllərin suvarılması və qulluğu"),
                ("Bağ-bağça işləri (mövsümi)", 800.00m, BillingPeriod.Quarterly, "Mövsümi əkilmə və bəzək işləri"),
            },
            8 => new List<(string, decimal, BillingPeriod, string?)> // Elektrik Təmiri FS
            {
                ("Elektrik təmiri (birdəfəlik iş)", 500.00m, BillingPeriod.OneTime, "Ümumi sahələrdə elektrik sistemlərinin təmiri"),
            },
            9 => new List<(string, decimal, BillingPeriod, string?)> // Santexnika Ustası FS
            {
                ("Santexnika təmiri (birdəfəlik iş)", 350.00m, BillingPeriod.OneTime, "Ümumi tualet və su sistemlərinin təmiri"),
            },
            10 => new List<(string, decimal, BillingPeriod, string?)> // İsmayıl Məmmədov
            {
                ("Kiçik təmir işləri (birdəfəlik)", 200.00m, BillingPeriod.OneTime, "Divar boyanması və xırda təmirlər"),
            },
            _ => new List<(string, decimal, BillingPeriod, string?)>
            {
                ("Ümumi xidmət (aylıq)", 100.00m, BillingPeriod.Monthly, "Standart aylıq xidmət"),
            }
        };
    }
}