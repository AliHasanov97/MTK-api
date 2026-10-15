using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MTK.DataSeeder.Seeders;
using MTK.Modules.Buildings.Infrastructure;
using MTK.Modules.Hr.Infrastructure;
using MTK.Modules.Payments.Infrastructure;
using MTK.Modules.Warehouse.Infrastructure;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        // API-dakı appsettings istifadə et (duplicate config yox!)
        // AppContext.BaseDirectory = bin/Debug/net9.0/ qovluğudur
        var apiDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "MTK.Api"));

        // Mühit API-nin launchSettings.json faylından götürülür (Production/Development).
        // ASPNETCORE_ENVIRONMENT artıq təyin olunubsa, o üstünlük təşkil edir.
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? ReadLaunchSettingsEnvironment(Path.Combine(apiDir, "Properties", "launchSettings.json"))
            ?? "Production";
        Console.WriteLine($"DataSeeder mühiti: {environmentName} (appsettings.{environmentName}.json)");

        config.AddJsonFile(Path.Combine(apiDir, "appsettings.json"), optional: false, reloadOnChange: true);
        config.AddJsonFile(Path.Combine(apiDir, $"appsettings.{environmentName}.json"), optional: true, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        // Fake services for CLI/Seeder (DbContext dependencies)
        services.AddScoped<MTK.Modules.Identity.Application.Abstractions.IUserContext, MTK.DataSeeder.Infrastructure.FakeUserContext>();
        services.AddScoped<MTK.Common.Application.Auditing.IAuditActorAccessor, MTK.DataSeeder.Infrastructure.FakeAuditActorAccessor>();
        services.AddScoped<MTK.Common.Application.Caching.ICacheService, MTK.DataSeeder.Infrastructure.FakeCacheService>();
        services.AddScoped<MTK.Common.Application.Messaging.IDateTimeProvider, MTK.DataSeeder.Infrastructure.FakeDateTimeProvider>();

        // Real AutoMapper (lazımdır çünki Payments module mapping istifadə edir)
        // AutoMapper 16+ API ilə manual configuration
        services.AddSingleton<AutoMapper.IConfigurationProvider>(sp =>
        {
            var loggerFactory = sp.GetRequiredService<Microsoft.Extensions.Logging.ILoggerFactory>();
            var mapperConfigExpression = new AutoMapper.MapperConfigurationExpression();
            mapperConfigExpression.AddMaps(typeof(MTK.Modules.Payments.Application.PaymentsMappingProfile).Assembly);
            mapperConfigExpression.AddMaps(typeof(MTK.Modules.Hr.Application.AssemblyReference).Assembly);
            return new AutoMapper.MapperConfiguration(mapperConfigExpression, loggerFactory);
        });
        services.AddScoped<AutoMapper.IMapper>(sp =>
            new AutoMapper.Mapper(sp.GetRequiredService<AutoMapper.IConfigurationProvider>()));

        // Outbox interceptor - real interceptor with logger (domain events ignore olunacaq, outbox table yoxdur CLI-də)
        services.AddScoped<MTK.Common.Infrastructure.Outbox.InsertOutboxMessagesInterceptor>();

        // Buildings Module
        services.AddBuildingsModule(configuration);

        // Payments Module
        services.AddPaymentsModule(configuration);

        // Warehouse Module
        services.AddWarehouseModule(configuration);

        // Hr Module
        services.AddHrModule(configuration);

        // Seeders
        services.AddScoped<BuildingSeeder>();
        services.AddScoped<OwnerSeeder>();
        services.AddScoped<ApartmentSeeder>();
        services.AddScoped<GarageSeeder>();
        services.AddScoped<OwnershipSeeder>();
        services.AddScoped<VendorSeeder>();
        services.AddScoped<ContractSeeder>();
        services.AddScoped<RateSeeder>();
        services.AddScoped<NomenclatureSeeder>();
        services.AddScoped<WarehouseStockSeeder>();
        services.AddScoped<ChargeSeeder>();
        services.AddScoped<OwnerPaymentSeeder>();
        services.AddScoped<VendorPaymentSeeder>();
        services.AddScoped<HrJobSeeder>();
        services.AddScoped<HrEducationalInstitutionSeeder>();
        services.AddScoped<HrCalendarDaySeeder>();
        services.AddScoped<HrEmployeeSeeder>();
    })
    .Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();

try { Console.Clear(); } catch { }
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("╔════════════════════════════════════════════════════════╗");
Console.WriteLine("║           MTK DATA SEEDER - İDARƏ PANELİ              ║");
Console.WriteLine("╚════════════════════════════════════════════════════════╝");
Console.ResetColor();
Console.WriteLine();

while (true)
{
    try
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("┌────────────────────── MENYU ──────────────────────────┐");
        Console.ResetColor();
        Console.WriteLine("│                                                        │");
        Console.WriteLine("│  1. 🌱 Bütün məlumatları seed et (Full Seed)          │");
        Console.WriteLine("│  2. 🏢 Yalnız binaları seed et                         │");
        Console.WriteLine("│  3. 👥 Yalnız sakinləri seed et                        │");
        Console.WriteLine("│  4. 🏠 Yalnız mənzilləri seed et                       │");
        Console.WriteLine("│  5. 🚗 Yalnız qarajları seed et                        │");
        Console.WriteLine("│  6. 🔗 Yalnız sakinlik əlaqələrini seed et             │");
        Console.WriteLine("│  7. 🏪 Yalnız tədarükçüləri seed et                    │");
        Console.WriteLine("│  8. 📄 Yalnız müqavilələri seed et                     │");
        Console.WriteLine("│  9. 💰 Yalnız tarifləri seed et                        │");
        Console.WriteLine("│ 10. 📦 Yalnız nomenklatur seed et                      │");
        Console.WriteLine("│ 11. 🏭 Yalnız anbar stoku seed et                      │");
        Console.WriteLine("│ 12. 💳 Yalnız borcları seed et                         │");
        Console.WriteLine("│ 13. 💵 Yalnız sakin ödənişləri seed et                 │");
        Console.WriteLine("│ 14. 💸 Yalnız vendor ödənişləri seed et                │");
        Console.WriteLine("│ 15. 💼 Yalnız HR vəzifələrini seed et                  │");
        Console.WriteLine("│ 16. 🎓 Yalnız HR təhsil ocaqlarını seed et             │");
        Console.WriteLine("│ 17. 📅 Yalnız HR təqvim günlərini seed et              │");
        Console.WriteLine("│ 18. 🧑‍💼 Yalnız HR işçilərini seed et                   │");
        Console.WriteLine("│ 19. 🗑️  Bazanı təmizlə (DROP DATABASE)                 │");
        Console.WriteLine("│ 20. 📊 Statistika göstər                               │");
        Console.WriteLine("│ 21. ❌ Çıxış                                            │");
        Console.WriteLine("│                                                        │");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("└────────────────────────────────────────────────────────┘");
        Console.ResetColor();
        Console.WriteLine();
        Console.Write("Seçiminizi edin (1-21): ");

        var choice = Console.ReadLine();
        Console.WriteLine();

        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;

        switch (choice)
        {
            case "1":
                await SeedAll(services, logger);
                break;
            case "2":
                await SeedBuildings(services, logger);
                break;
            case "3":
                await SeedOwners(services, logger);
                break;
            case "4":
                await SeedApartments(services, logger);
                break;
            case "5":
                await SeedGarages(services, logger);
                break;
            case "6":
                await SeedOwnerships(services, logger);
                break;
            case "7":
                await SeedVendors(services, logger);
                break;
            case "8":
                await SeedContracts(services, logger);
                break;
            case "9":
                await SeedRates(services, logger);
                break;
            case "10":
                await SeedNomenclatures(services, logger);
                break;
            case "11":
                await SeedWarehouseStock(services, logger);
                break;
            case "12":
                await SeedCharges(services, logger);
                break;
            case "13":
                await SeedOwnerPayments(services, logger);
                break;
            case "14":
                await SeedVendorPayments(services, logger);
                break;
            case "15":
                await SeedHrJobs(services, logger);
                break;
            case "16":
                await SeedHrEducationalInstitutions(services, logger);
                break;
            case "17":
                await SeedHrCalendarDays(services, logger);
                break;
            case "18":
                await SeedHrEmployees(services, logger);
                break;
            case "19":
                await DropDatabase(logger);
                break;
            case "20":
                await ShowStatistics(services, logger);
                break;
            case "21":
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("👋 Görüşənədək!");
                Console.ResetColor();
                return 0;
            default:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ Yanlış seçim! Zəhmət olmasa 1-21 arası seçin.");
                Console.ResetColor();
                break;
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("Davam etmək üçün bir düyməyə basın...");
        Console.ResetColor();
        Console.ReadKey();
        try { Console.Clear(); } catch { }
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔════════════════════════════════════════════════════════╗");
        Console.WriteLine("║           MTK DATA SEEDER - İDARƏ PANELİ              ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Əməliyyat zamanı xəta baş verdi!");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"❌ Xəta: {ex.Message}");
        Console.ResetColor();
    }
}

static async Task SeedAll(IServiceProvider services, ILogger logger)
{
    logger.LogInformation("🌱 Bütün məlumatlar seed edilir...");
    logger.LogInformation("");

    await SeedBuildings(services, logger);
    await SeedOwners(services, logger);
    await SeedApartments(services, logger);
    await SeedGarages(services, logger);
    await SeedOwnerships(services, logger);
    await SeedVendors(services, logger);
    await SeedContracts(services, logger);
    await SeedRates(services, logger);
    await SeedNomenclatures(services, logger);
    await SeedWarehouseStock(services, logger);
    await SeedCharges(services, logger);
    await SeedOwnerPayments(services, logger);
    await SeedVendorPayments(services, logger);
    await SeedHrJobs(services, logger);
    await SeedHrEducationalInstitutions(services, logger);
    await SeedHrCalendarDays(services, logger);
    await SeedHrEmployees(services, logger);

    logger.LogInformation("🎉 Bütün seed prosesi uğurla tamamlandı!");
    logger.LogInformation("");
    logger.LogInformation("📊 Statistika:");
    logger.LogInformation("  - 3 blok (17 mərtəbə × 6 mənzil)");
    logger.LogInformation("  - 306 mənzil (102 × 3)");
    logger.LogInformation("  - 30 qaraj");
    logger.LogInformation("  - ~245 sakin (qeydiyyatlı)");
    logger.LogInformation("  - ~10 tədarükçü");
    logger.LogInformation("  - ~10 müqavilə (xidmətlərlə)");
    logger.LogInformation("  - 4 tarif (mənzil və qaraj)");
    logger.LogInformation("  - ~25 nomenklatura");
    logger.LogInformation("  - ~25 anbar stoku");
    logger.LogInformation("  - Son 3 ay üçün borclar");
    logger.LogInformation("  - Sakin və vendor ödənişləri");
    logger.LogInformation("  - HR: vəzifələr, təhsil ocaqları, təqvim günləri, ~30 işçi (iş/təhsil tarixçəsi ilə)");
    logger.LogInformation("  - HR: məzuniyyət (və digər) ərizə/əmrləri QƏSDƏN seed edilmir");
    logger.LogInformation("");
    logger.LogInformation("✅ Integration events avtomatik Payments module-a göndərildi!");
    logger.LogInformation("✅ Cache invalidation işlədi!");
}

static async Task SeedBuildings(IServiceProvider services, ILogger logger)
{
    logger.LogInformation("📦 Buildings seed edilir...");
    var buildingSeeder = services.GetRequiredService<BuildingSeeder>();
    await buildingSeeder.SeedAsync();
    logger.LogInformation("✅ Buildings seed tamamlandı!");
    logger.LogInformation("");
}

static async Task SeedOwners(IServiceProvider services, ILogger logger)
{
    logger.LogInformation("👥 Owners (Sakinlər) seed edilir...");
    var ownerSeeder = services.GetRequiredService<OwnerSeeder>();
    await ownerSeeder.SeedAsync();
    logger.LogInformation("✅ Owners seed tamamlandı!");
    logger.LogInformation("");
}

static async Task SeedApartments(IServiceProvider services, ILogger logger)
{
    logger.LogInformation("🏠 Apartments (Mənzillər) seed edilir...");
    var apartmentSeeder = services.GetRequiredService<ApartmentSeeder>();
    await apartmentSeeder.SeedAsync();
    logger.LogInformation("✅ Apartments seed tamamlandı!");
    logger.LogInformation("");
}

static async Task SeedGarages(IServiceProvider services, ILogger logger)
{
    logger.LogInformation("🚗 Garages (Qarajlar) seed edilir...");
    var garageSeeder = services.GetRequiredService<GarageSeeder>();
    await garageSeeder.SeedAsync();
    logger.LogInformation("✅ Garages seed tamamlandı!");
    logger.LogInformation("");
}

static async Task SeedOwnerships(IServiceProvider services, ILogger logger)
{
    logger.LogInformation("🔗 Ownerships (Sakinlik təyin edilir)...");
    var ownershipSeeder = services.GetRequiredService<OwnershipSeeder>();
    await ownershipSeeder.SeedAsync();
    logger.LogInformation("✅ Ownerships seed tamamlandı!");
    logger.LogInformation("");
}

static async Task SeedVendors(IServiceProvider services, ILogger logger)
{
    logger.LogInformation("🏪 Vendors (Tədarükçülər) seed edilir...");
    var vendorSeeder = services.GetRequiredService<VendorSeeder>();
    await vendorSeeder.SeedAsync();
    logger.LogInformation("✅ Vendors seed tamamlandı!");
    logger.LogInformation("");
}

static async Task SeedContracts(IServiceProvider services, ILogger logger)
{
    logger.LogInformation("📄 Contracts (Müqavilələr) seed edilir...");
    var contractSeeder = services.GetRequiredService<ContractSeeder>();
    await contractSeeder.SeedAsync();
    logger.LogInformation("✅ Contracts seed tamamlandı!");
    logger.LogInformation("");
}

static async Task SeedRates(IServiceProvider services, ILogger logger)
{
    logger.LogInformation("💰 Rates (Tariflər) seed edilir...");
    var rateSeeder = services.GetRequiredService<RateSeeder>();
    await rateSeeder.SeedAsync();
    logger.LogInformation("✅ Rates seed tamamlandı!");
    logger.LogInformation("");
}

static async Task SeedNomenclatures(IServiceProvider services, ILogger logger)
{
    logger.LogInformation("📦 Nomenclatures (Nomenklaturalar) seed edilir...");
    var nomenclatureSeeder = services.GetRequiredService<NomenclatureSeeder>();
    await nomenclatureSeeder.SeedAsync();
    logger.LogInformation("✅ Nomenclatures seed tamamlandı!");
    logger.LogInformation("");
}

static async Task SeedWarehouseStock(IServiceProvider services, ILogger logger)
{
    logger.LogInformation("🏭 Warehouse Stock (Anbar stoku) seed edilir...");
    var warehouseStockSeeder = services.GetRequiredService<WarehouseStockSeeder>();
    await warehouseStockSeeder.SeedAsync();
    logger.LogInformation("✅ Warehouse Stock seed tamamlandı!");
    logger.LogInformation("");
}

static async Task SeedCharges(IServiceProvider services, ILogger logger)
{
    logger.LogInformation("💳 Charges (Borclar) seed edilir...");
    var chargeSeeder = services.GetRequiredService<ChargeSeeder>();
    await chargeSeeder.SeedAsync();
    logger.LogInformation("✅ Charges seed tamamlandı!");
    logger.LogInformation("");
}

static async Task SeedOwnerPayments(IServiceProvider services, ILogger logger)
{
    logger.LogInformation("💵 Owner Payments (Sakin ödənişləri) seed edilir...");
    var ownerPaymentSeeder = services.GetRequiredService<OwnerPaymentSeeder>();
    await ownerPaymentSeeder.SeedAsync();
    logger.LogInformation("✅ Owner Payments seed tamamlandı!");
    logger.LogInformation("");
}

static async Task SeedVendorPayments(IServiceProvider services, ILogger logger)
{
    logger.LogInformation("💸 Vendor Payments (Vendor ödənişləri) seed edilir...");
    var vendorPaymentSeeder = services.GetRequiredService<VendorPaymentSeeder>();
    await vendorPaymentSeeder.SeedAsync();
    logger.LogInformation("✅ Vendor Payments seed tamamlandı!");
    logger.LogInformation("");
}

static async Task SeedHrJobs(IServiceProvider services, ILogger logger)
{
    logger.LogInformation("💼 HR vəzifələri seed edilir...");
    var jobSeeder = services.GetRequiredService<HrJobSeeder>();
    await jobSeeder.SeedAsync();
    logger.LogInformation("✅ HR vəzifələri seed tamamlandı!");
    logger.LogInformation("");
}

static async Task SeedHrEducationalInstitutions(IServiceProvider services, ILogger logger)
{
    logger.LogInformation("🎓 HR təhsil ocaqları seed edilir...");
    var institutionSeeder = services.GetRequiredService<HrEducationalInstitutionSeeder>();
    await institutionSeeder.SeedAsync();
    logger.LogInformation("✅ HR təhsil ocaqları seed tamamlandı!");
    logger.LogInformation("");
}

static async Task SeedHrCalendarDays(IServiceProvider services, ILogger logger)
{
    logger.LogInformation("📅 HR təqvim günləri seed edilir...");
    var calendarDaySeeder = services.GetRequiredService<HrCalendarDaySeeder>();
    await calendarDaySeeder.SeedAsync();
    logger.LogInformation("✅ HR təqvim günləri seed tamamlandı!");
    logger.LogInformation("");
}

static async Task SeedHrEmployees(IServiceProvider services, ILogger logger)
{
    logger.LogInformation("🧑‍💼 HR işçiləri seed edilir (məzuniyyət ərizə/əmrləri seed edilmir)...");
    var employeeSeeder = services.GetRequiredService<HrEmployeeSeeder>();
    await employeeSeeder.SeedAsync();
    logger.LogInformation("✅ HR işçiləri seed tamamlandı!");
    logger.LogInformation("");
}

static async Task DropDatabase(ILogger logger)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("⚠️  DİQQƏT! Bu əməliyyat bütün məlumatları silir!");
    Console.ResetColor();
    Console.Write("Davam etmək üçün 'EVET' yazın: ");
    var confirmation = Console.ReadLine();

    if (confirmation?.ToUpper() == "EVET")
    {
        logger.LogInformation("🗑️  Baza təmizlənir...");
        logger.LogWarning("⚠️  Bu funksiya hələlik əl ilə edilməlidir: dotnet ef database drop");
        logger.LogInformation("");
    }
    else
    {
        logger.LogInformation("❌ Əməliyyat ləğv edildi.");
    }

    await Task.CompletedTask;
}

static async Task ShowStatistics(IServiceProvider services, ILogger logger)
{
    var buildingRepo = services.GetRequiredService<MTK.Modules.Buildings.Domain.Repositories.IBuildingRepository>();
    var ownerRepo = services.GetRequiredService<MTK.Modules.Buildings.Domain.Repositories.IOwnerRepository>();
    var apartmentRepo = services.GetRequiredService<MTK.Modules.Buildings.Domain.Repositories.IApartmentRepository>();
    var garageRepo = services.GetRequiredService<MTK.Modules.Buildings.Domain.Repositories.IGarageRepository>();

    // Payments repositories
    var vendorRepo = services.GetRequiredService<MTK.Modules.Payments.Domain.Repositories.IVendorRepository>();
    var contractRepo = services.GetRequiredService<MTK.Modules.Payments.Domain.Repositories.IContractRepository>();

    var buildings = await buildingRepo.ListAsync();
    var owners = await ownerRepo.ListAsync();
    var apartments = await apartmentRepo.ListAsync();
    var garages = await garageRepo.ListAsync();
    var vendors = await vendorRepo.ListAsync();
    var contracts = await contractRepo.ListAsync();

    var assignedApartments = apartments.Count(a => a.CurrentOwnerId.HasValue);
    var assignedGarages = garages.Count(g => g.OwnerId.HasValue);
    var activeContracts = contracts.Count(c => c.Status == MTK.Modules.Payments.Domain.Contracts.ContractStatus.Active);

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("╔════════════════════════════════════════════════════════╗");
    Console.WriteLine("║                   📊 STATİSTİKA                        ║");
    Console.WriteLine("╚════════════════════════════════════════════════════════╝");
    Console.ResetColor();
    Console.WriteLine();
    Console.WriteLine($"🏢 Binalar: {buildings.Count()} ədəd");
    Console.WriteLine($"👥 Sakinlər: {owners.Count()} ədəd (qeydiyyatlı)");
    Console.WriteLine($"🏠 Mənzillər: {apartments.Count()} ədəd ({assignedApartments} təyinatlı, {apartments.Count() - assignedApartments} boş)");
    Console.WriteLine($"🚗 Qarajlar: {garages.Count()} ədəd ({assignedGarages} təyinatlı, {garages.Count() - assignedGarages} boş)");
    Console.WriteLine($"🏪 Tədarükçülər: {vendors.Count()} ədəd");
    Console.WriteLine($"📄 Müqavilələr: {contracts.Count()} ədəd ({activeContracts} aktiv)");
    Console.WriteLine();

    foreach (var building in buildings)
    {
        var buildingApartments = apartments.Count(a => a.BuildingId == building.Id);
        var buildingAssigned = apartments.Count(a => a.BuildingId == building.Id && a.CurrentOwnerId.HasValue);
        Console.WriteLine($"  🏢 {building.Name}: {buildingApartments} mənzil ({buildingAssigned} təyinatlı)");
    }

    logger.LogInformation("");
}

static string? ReadLaunchSettingsEnvironment(string launchSettingsPath)
{
    if (!File.Exists(launchSettingsPath))
        return null;

    using var doc = System.Text.Json.JsonDocument.Parse(
        File.ReadAllText(launchSettingsPath),
        new System.Text.Json.JsonDocumentOptions { CommentHandling = System.Text.Json.JsonCommentHandling.Skip, AllowTrailingCommas = true });

    if (!doc.RootElement.TryGetProperty("profiles", out var profiles))
        return null;

    foreach (var profile in profiles.EnumerateObject())
    {
        if (profile.Value.TryGetProperty("environmentVariables", out var vars)
            && vars.TryGetProperty("ASPNETCORE_ENVIRONMENT", out var env)
            && env.ValueKind == System.Text.Json.JsonValueKind.String)
        {
            return env.GetString();
        }
    }

    return null;
}
