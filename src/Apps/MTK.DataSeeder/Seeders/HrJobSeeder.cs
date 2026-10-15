using MediatR;
using Microsoft.Extensions.Logging;
using MTK.Modules.Hr.Application.Jobs.AddJob;
using MTK.Modules.Hr.Domain.Jobs;

namespace MTK.DataSeeder.Seeders;

/// <summary>
/// HR vəzifələrini (Jobs) seed edir — digər HR seederlər (ilk növbədə HrEmployeeSeeder) buna əsaslanır.
/// </summary>
public sealed class HrJobSeeder
{
    private readonly ISender _sender;
    private readonly ILogger<HrJobSeeder> _logger;
    private readonly IJobRepository _jobRepository;

    public static readonly List<Guid> JobIds = new();

    private static readonly string[] JobNames =
    {
        "Baş direktor",
        "Direktor müavini",
        "Baş mühasib",
        "Mühasib",
        "Kadrlar şöbəsinin müdiri",
        "Kadrlar üzrə mütəxəssis",
        "Hüquqşünas",
        "Bina meneceri",
        "Komendant",
        "Santexnik",
        "Elektrik",
        "Təmizlikçi",
        "Təhlükəsizlik mühafizəçisi",
        "Anbardar",
        "Kargüzar",
        "Katib",
        "Sürücü",
        "IT mütəxəssisi",
    };

    public HrJobSeeder(ISender sender, ILogger<HrJobSeeder> logger, IJobRepository jobRepository)
    {
        _sender = sender;
        _logger = logger;
        _jobRepository = jobRepository;
    }

    public async Task SeedAsync()
    {
        var existingJobs = await _jobRepository.ListAsync();
        JobIds.Clear();
        JobIds.AddRange(existingJobs.Select(j => j.Id));

        if (JobIds.Any())
        {
            _logger.LogInformation("  ℹ️  Bazada artıq {Count} vəzifə mövcuddur", JobIds.Count);
            return;
        }

        foreach (var name in JobNames)
        {
            var result = await _sender.Send(new AddJobCommand { Name = name });

            if (result.IsSuccess)
            {
                JobIds.Add(result.Value.Id);
            }
            else
            {
                _logger.LogError("  ❌ Vəzifə yaradıla bilmədi ({Name}): {Error}", name, result.Error);
            }
        }

        _logger.LogInformation("  📊 Cəmi {Count} vəzifə yaradıldı", JobIds.Count);
    }
}
