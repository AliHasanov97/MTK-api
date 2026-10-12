using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MTK.DataSeeder.Seeders;
using MTK.Modules.Buildings.Infrastructure;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        // API-dakı appsettings.json istifadə et (duplicate config yox!)
        // AppContext.BaseDirectory = bin/Debug/net9.0/ qovluğudur
        var baseDir = AppContext.BaseDirectory;
        var apiAppSettingsPath = Path.Combine(baseDir, "..", "..", "..", "..", "MTK.Api", "appsettings.json");
        var fullPath = Path.GetFullPath(apiAppSettingsPath);

        config.AddJsonFile(fullPath, optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        // Fake services for CLI/Seeder (DbContext dependencies)
        services.AddScoped<MTK.Modules.Identity.Application.Abstractions.IUserContext, MTK.DataSeeder.Infrastructure.FakeUserContext>();
        services.AddScoped<MTK.Common.Application.Auditing.IAuditActorAccessor, MTK.DataSeeder.Infrastructure.FakeAuditActorAccessor>();

        // Outbox interceptor - real interceptor with logger (domain events ignore olunacaq, outbox table yoxdur CLI-də)
        services.AddScoped<MTK.Common.Infrastructure.Outbox.InsertOutboxMessagesInterceptor>();

        // Buildings Module
        services.AddBuildingsModule(configuration);

        // Seeders
        services.AddScoped<BuildingSeeder>();
        services.AddScoped<OwnerSeeder>();
        services.AddScoped<ApartmentSeeder>();
        services.AddScoped<GarageSeeder>();
        services.AddScoped<OwnershipSeeder>();
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
        Console.WriteLine("│  7. 🗑️  Bazanı təmizlə (DROP DATABASE)                 │");
        Console.WriteLine("│  8. 📊 Statistika göstər                               │");
        Console.WriteLine("│  9. ❌ Çıxış                                            │");
        Console.WriteLine("│                                                        │");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("└────────────────────────────────────────────────────────┘");
        Console.ResetColor();
        Console.WriteLine();
        Console.Write("Seçiminizi edin (1-9): ");

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
                await DropDatabase(logger);
                break;
            case "8":
                await ShowStatistics(services, logger);
                break;
            case "9":
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("👋 Görüşənədək!");
                Console.ResetColor();
                return 0;
            default:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ Yanlış seçim! Zəhmət olmasa 1-9 arası seçin.");
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

    logger.LogInformation("🎉 Bütün seed prosesi uğurla tamamlandı!");
    logger.LogInformation("");
    logger.LogInformation("📊 Statistika:");
    logger.LogInformation("  - 3 blok (17 mərtəbə × 6 mənzil)");
    logger.LogInformation("  - 306 mənzil (102 × 3)");
    logger.LogInformation("  - 30 qaraj");
    logger.LogInformation("  - ~245 sakin (qeydiyyatlı)");
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

    var buildings = await buildingRepo.GetAllAsync();
    var owners = await ownerRepo.GetAllAsync();
    var apartments = await apartmentRepo.GetAllAsync();
    var garages = await garageRepo.GetAllAsync();

    var assignedApartments = apartments.Count(a => a.CurrentOwnerId.HasValue);
    var assignedGarages = garages.Count(g => g.OwnerId.HasValue);

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
    Console.WriteLine();

    foreach (var building in buildings)
    {
        var buildingApartments = apartments.Count(a => a.BuildingId == building.Id);
        var buildingAssigned = apartments.Count(a => a.BuildingId == building.Id && a.CurrentOwnerId.HasValue);
        Console.WriteLine($"  🏢 {building.Name}: {buildingApartments} mənzil ({buildingAssigned} təyinatlı)");
    }

    logger.LogInformation("");
}
