using Bogus;
using MediatR;
using Microsoft.Extensions.Logging;
using MTK.Modules.Buildings.Application.Apartments.Commands.CreateApartment;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.DataSeeder.Seeders;

/// <summary>
/// 3 blokda hər birində 102 mənzil (17 mərtəbə × 6 mənzil) = 306 mənzil seed edir
/// </summary>
public sealed class ApartmentSeeder
{
    private readonly ISender _sender;
    private readonly ILogger<ApartmentSeeder> _logger;
    private readonly IApartmentRepository _apartmentRepository;

    public static readonly List<Guid> ApartmentIds = new();

    public ApartmentSeeder(ISender sender, ILogger<ApartmentSeeder> logger, IApartmentRepository apartmentRepository)
    {
        _sender = sender;
        _logger = logger;
        _apartmentRepository = apartmentRepository;
    }

    public async Task SeedAsync()
    {
        // Əvvəlcə bütün mövcud mənzilləri bazadan oxu
        var existingApartments = await _apartmentRepository.ListAsync();
        ApartmentIds.Clear();
        ApartmentIds.AddRange(existingApartments.Select(a => a.Id));

        if (ApartmentIds.Any())
        {
            _logger.LogInformation("  ℹ️  Bazada artıq {Count} mənzil mövcuddur", ApartmentIds.Count);
        }

        var faker = new Faker();

        foreach (var buildingId in BuildingSeeder.BuildingIds)
        {
            var buildingIndex = BuildingSeeder.BuildingIds.IndexOf(buildingId) + 1;
            _logger.LogInformation("  🏢 Blok {Index} üçün mənzillər yaradılır...", buildingIndex);

            // Hər blokda 102 mənzil (17 mərtəbə × 6 mənzil)
            for (int floor = 1; floor <= 17; floor++)
            {
                for (int apt = 1; apt <= 6; apt++)
                {
                    var apartmentNumber = $"{floor}{apt}"; // 11, 12, ..., 16, 21, ...
                    var areaSquareMeters = faker.Random.Decimal(45, 150); // 45-150 m²
                    var roomCount = faker.Random.Int(1, 4); // 1-4 otaq

                    var command = new CreateApartmentCommand(
                        buildingId,
                        apartmentNumber,
                        floor,
                        areaSquareMeters,
                        roomCount);

                    var result = await _sender.Send(command);

                    if (result.IsSuccess)
                    {
                        ApartmentIds.Add(result.Value);
                    }
                    else
                    {
                        _logger.LogError("  ❌ Mənzil yaradıla bilmədi: {Error}", result.Error);
                    }
                }
            }

            _logger.LogInformation("  ✅ Blok {Index} üçün 102 mənzil yaradıldı", buildingIndex);
        }

        _logger.LogInformation("  📊 Cəmi {Count} mənzil yaradıldı", ApartmentIds.Count);
    }
}
