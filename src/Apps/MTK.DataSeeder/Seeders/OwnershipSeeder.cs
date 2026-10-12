using Bogus;
using MediatR;
using Microsoft.Extensions.Logging;
using MTK.Modules.Buildings.Application.Apartments.Commands.AssignOwnerToApartment;
using MTK.Modules.Buildings.Application.Garages.Commands.AssignOwnerToGarage;

namespace MTK.DataSeeder.Seeders;

/// <summary>
/// Sakinlərə mənzil və qaraj təyin edir
/// - ~80% mənzillərdə sakin var (~245 mənzil)
/// - Bəzi sakinlər 2-3 mənzilə sahib (~20 sakin)
/// - Qarajlar random təyin olunur
/// </summary>
public sealed class OwnershipSeeder
{
    private readonly ISender _sender;
    private readonly ILogger<OwnershipSeeder> _logger;

    public OwnershipSeeder(ISender sender, ILogger<OwnershipSeeder> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        var faker = new Faker();

        var availableApartments = new List<Guid>(ApartmentSeeder.ApartmentIds);
        var availableGarages = new List<Guid>(GarageSeeder.GarageIds);
        var availableOwners = new List<Guid>(OwnerSeeder.OwnerIds);

        int apartmentsAssigned = 0;
        int garagesAssigned = 0;

        // 1. Hər sakinə ən azı 1 mənzil təyin et
        foreach (var ownerId in availableOwners.Take(245)) // ~80% doluluk
        {
            if (availableApartments.Count == 0) break;

            var apartmentId = faker.PickRandom(availableApartments);
            availableApartments.Remove(apartmentId);

            var command = new AssignOwnerToApartmentCommand
            {
                ApartmentId = apartmentId,
                OwnerId = ownerId
            };

            var result = await _sender.Send(command);
            if (result.IsSuccess)
            {
                apartmentsAssigned++;
            }
            else
            {
                _logger.LogError("  ❌ Mənzilə sakin təyin edilə bilmədi: Apartment={ApartmentId}, Owner={OwnerId}, Error={Error}",
                    apartmentId, ownerId, result.Error);
            }
        }

        _logger.LogInformation("  ✅ {Count} mənzilə sakin təyin edildi", apartmentsAssigned);

        // 2. Bəzi sakinlərə 2-ci və 3-cü mənzil ver (~20 sakin, 2-3 mənzil)
        var richOwners = availableOwners.Take(20).ToList();
        foreach (var ownerId in richOwners)
        {
            // 2-ci mənzil
            if (availableApartments.Count > 0 && faker.Random.Bool(0.8f)) // 80% ehtimal
            {
                var apartmentId = faker.PickRandom(availableApartments);
                availableApartments.Remove(apartmentId);

                var command = new AssignOwnerToApartmentCommand
                {
                    ApartmentId = apartmentId,
                    OwnerId = ownerId
                };

                var result = await _sender.Send(command);
                if (result.IsSuccess)
                {
                    apartmentsAssigned++;
                }
            }

            // 3-cü mənzil (daha az ehtimal)
            if (availableApartments.Count > 0 && faker.Random.Bool(0.3f)) // 30% ehtimal
            {
                var apartmentId = faker.PickRandom(availableApartments);
                availableApartments.Remove(apartmentId);

                var command = new AssignOwnerToApartmentCommand
                {
                    ApartmentId = apartmentId,
                    OwnerId = ownerId
                };

                var result = await _sender.Send(command);
                if (result.IsSuccess)
                {
                    apartmentsAssigned++;
                }
            }
        }

        _logger.LogInformation("  ✅ Cəmi {Count} mənzil-sakin əlaqəsi yaradıldı", apartmentsAssigned);

        // 3. Qarajları random sakinlərə təyin et
        foreach (var garageId in availableGarages)
        {
            if (availableOwners.Count == 0) break;

            var ownerId = faker.PickRandom(availableOwners);

            var command = new AssignOwnerToGarageCommand
            {
                GarageId = garageId,
                OwnerId = ownerId
            };

            var result = await _sender.Send(command);
            if (result.IsSuccess)
            {
                garagesAssigned++;
            }
        }

        _logger.LogInformation("  ✅ {Count} qaraj-sakin əlaqəsi yaradıldı", garagesAssigned);
        _logger.LogInformation("");
        _logger.LogInformation("  📊 Statistika:");
        _logger.LogInformation("    - Mənzillər: {Total} cəmi, {Assigned} təyin edilib, {Empty} boş",
            ApartmentSeeder.ApartmentIds.Count, apartmentsAssigned,
            ApartmentSeeder.ApartmentIds.Count - apartmentsAssigned);
        _logger.LogInformation("    - Qarajlar: {Total} cəmi, {Assigned} təyin edilib",
            GarageSeeder.GarageIds.Count, garagesAssigned);
    }
}
