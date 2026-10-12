using Bogus;
using MediatR;
using Microsoft.Extensions.Logging;
using MTK.Modules.Buildings.Application.Garages.Commands.CreateGarage;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.DataSeeder.Seeders;

/// <summary>
/// 30 qaraj seed edir
/// </summary>
public sealed class GarageSeeder
{
    private readonly ISender _sender;
    private readonly ILogger<GarageSeeder> _logger;
    private readonly IGarageRepository _garageRepository;

    public static readonly List<Guid> GarageIds = new();

    public GarageSeeder(ISender sender, ILogger<GarageSeeder> logger, IGarageRepository garageRepository)
    {
        _sender = sender;
        _logger = logger;
        _garageRepository = garageRepository;
    }

    public async Task SeedAsync()
    {
        // Əvvəlcə bütün mövcud qarajları bazadan oxu
        var existingGarages = await _garageRepository.GetAllAsync();
        GarageIds.Clear();
        GarageIds.AddRange(existingGarages.Select(g => g.Id));

        if (GarageIds.Any())
        {
            _logger.LogInformation("  ℹ️  Bazada artıq {Count} qaraj mövcuddur", GarageIds.Count);
        }

        var faker = new Faker();

        // 30 qaraj yarat
        for (int i = 1; i <= 30; i++)
        {
            var garageNumber = $"G-{i:D3}"; // G-001, G-002, ...
            var garageType = faker.PickRandom(new[] { "UndergroundParking", "OpenParking", "ClosedGarage" });

            var command = new CreateGarageCommand(
                OwnerId: null, // Başda owner yoxdur
                garageNumber,
                garageType,
                Description: null);

            var result = await _sender.Send(command);

            if (result.IsSuccess)
            {
                GarageIds.Add(result.Value);
            }
            else
            {
                _logger.LogError("  ❌ Qaraj yaradıla bilmədi: {Error}", result.Error);
            }
        }

        _logger.LogInformation("  📊 Cəmi {Count} qaraj yaradıldı", GarageIds.Count);
    }
}
