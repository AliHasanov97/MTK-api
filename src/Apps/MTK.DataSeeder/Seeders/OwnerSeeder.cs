using Bogus;
using MediatR;
using Microsoft.Extensions.Logging;
using MTK.Modules.Buildings.Application.Owners.Commands.CreateOwner;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.DataSeeder.Seeders;

/// <summary>
/// ~300 sakin (owner) seed edir
/// </summary>
public sealed class OwnerSeeder
{
    private readonly ISender _sender;
    private readonly ILogger<OwnerSeeder> _logger;
    private readonly IOwnerRepository _ownerRepository;

    public static readonly List<Guid> OwnerIds = new();

    public OwnerSeeder(ISender sender, ILogger<OwnerSeeder> logger, IOwnerRepository ownerRepository)
    {
        _sender = sender;
        _logger = logger;
        _ownerRepository = ownerRepository;
    }

    public async Task SeedAsync()
    {
        // Əvvəlcə bütün mövcud sakinləri bazadan oxu
        var existingOwners = await _ownerRepository.GetAllAsync();
        OwnerIds.Clear();
        OwnerIds.AddRange(existingOwners.Select(o => o.Id)); // Owner.Id istifadə et, UserId deyil!

        if (OwnerIds.Any())
        {
            _logger.LogInformation("  ℹ️  Bazada artıq {Count} sakin mövcuddur", OwnerIds.Count);
        }

        var faker = new Faker("tr"); // Türk adları (Bogus az_AZ dəstəkləmir)

        // 306 mənzil var, ~80% mənzillərdə sakin olsun = ~245 sakin
        // Bəzi sakinlər 2-3 mənzilə sahib ola bilər
        var ownerCount = 245;

        for (int i = 0; i < ownerCount; i++)
        {
            var userId = Guid.NewGuid(); // Fake user ID (real sistemdə Identity moduldan gələcək)
            var firstName = faker.Name.FirstName();
            var lastName = faker.Name.LastName();
            var phone = faker.Phone.PhoneNumber("+994##-###-##-##");
            var email = faker.Internet.Email(firstName, lastName).ToLower();

            var command = new CreateOwnerCommand(userId, firstName, lastName, phone, email);
            var result = await _sender.Send(command);

            if (result.IsSuccess)
            {
                // Owner yaradıldıqdan sonra, Owner.Id-ni almaq üçün bazadan oxu
                var createdOwner = await _ownerRepository.GetByUserIdAsync(userId);
                if (createdOwner is not null)
                {
                    OwnerIds.Add(createdOwner.Id); // Owner.Id istifadə et!
                }

                if ((i + 1) % 50 == 0)
                {
                    _logger.LogInformation("  ✅ {Count} sakin yaradıldı...", i + 1);
                }
            }
            else
            {
                _logger.LogError("  ❌ Sakin yaradıla bilmədi: {Error}", result.Error);
            }
        }

        _logger.LogInformation("  📊 Cəmi {Count} sakin yaradıldı", OwnerIds.Count);
    }
}
