using Bogus;
using MediatR;
using Microsoft.Extensions.Logging;
using MTK.Modules.Buildings.Application.Buildings.Commands.CreateBuilding;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.DataSeeder.Seeders;

/// <summary>
/// 3 blok seed edir (hər birində 17 mərtəbə × 6 mənzil = 102 mənzil olacaq)
/// </summary>
public sealed class BuildingSeeder
{
    private readonly ISender _sender;
    private readonly ILogger<BuildingSeeder> _logger;
    private readonly IBuildingRepository _buildingRepository;

    public static readonly List<Guid> BuildingIds = new();

    public BuildingSeeder(ISender sender, ILogger<BuildingSeeder> logger, IBuildingRepository buildingRepository)
    {
        _sender = sender;
        _logger = logger;
        _buildingRepository = buildingRepository;
    }

    public async Task SeedAsync()
    {
        // Əvvəlcə bütün mövcud binaları bazadan oxu
        var existingBuildings = await _buildingRepository.GetAllAsync();
        BuildingIds.Clear();
        BuildingIds.AddRange(existingBuildings.Select(b => b.Id));

        if (BuildingIds.Any())
        {
            _logger.LogInformation("  ℹ️  Bazada artıq {Count} bina mövcuddur", BuildingIds.Count);
        }

        var buildings = new[]
        {
            new
            {
                Name = "Yasamal Residence A",
                Street = "H.Əliyev prospekti 123",
                City = "Bakı",
                District = "Yasamal rayonu",
                TotalFloors = 17,
                ApartmentsPerFloor = 6
            },
            new
            {
                Name = "Yasamal Residence B",
                Street = "H.Əliyev prospekti 125",
                City = "Bakı",
                District = "Yasamal rayonu",
                TotalFloors = 17,
                ApartmentsPerFloor = 6
            },
            new
            {
                Name = "Yasamal Residence C",
                Street = "H.Əliyev prospekti 127",
                City = "Bakı",
                District = "Yasamal rayonu",
                TotalFloors = 17,
                ApartmentsPerFloor = 6
            }
        };

        foreach (var building in buildings)
        {
            var command = new CreateBuildingCommand(
                building.Name,
                building.Street,
                building.City,
                building.District,
                building.TotalFloors,
                building.ApartmentsPerFloor,
                Description: null);

            var result = await _sender.Send(command);

            if (result.IsSuccess)
            {
                BuildingIds.Add(result.Value);
                _logger.LogInformation("  ✅ Bina yaradıldı: {Name}", building.Name);
            }
            else
            {
                _logger.LogError("  ❌ Bina yaradıla bilmədi: {Error}", result.Error);
            }
        }

        _logger.LogInformation("  📊 Cəmi {Count} bina yaradıldı", BuildingIds.Count);
    }
}
