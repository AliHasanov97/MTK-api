using MediatR;
using Microsoft.Extensions.Logging;
using MTK.Modules.Payments.Application.Rates.Commands.CreateRate;
using MTK.Modules.Payments.Domain.PropertyOwnerships;
using MTK.Modules.Payments.Domain.Rates;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.DataSeeder.Seeders;

/// <summary>
/// Tariflər (rates) seed edir
/// </summary>
public sealed class RateSeeder
{
    private readonly ISender _sender;
    private readonly ILogger<RateSeeder> _logger;
    private readonly IRateRepository _rateRepository;

    public static readonly List<Guid> RateIds = new();

    public RateSeeder(ISender sender, ILogger<RateSeeder> logger, IRateRepository rateRepository)
    {
        _sender = sender;
        _logger = logger;
        _rateRepository = rateRepository;
    }

    public async Task SeedAsync()
    {
        // Əvvəlcə bütün mövcud tarifləri bazadan oxu
        var existingRates = await _rateRepository.GetActiveRatesAsync();
        RateIds.Clear();
        RateIds.AddRange(existingRates.Select(r => r.Id));

        if (RateIds.Any())
        {
            _logger.LogInformation("  ℹ️  Bazada artıq {Count} tarif mövcuddur", RateIds.Count);
            return;
        }

        var currentYear = DateTimeOffset.UtcNow.Year;
        var effectiveFrom = new DateTimeOffset(currentYear, 1, 1, 0, 0, 0, TimeSpan.Zero);

        // Tariflər siyahısı
        var rates = new List<(RateType RateType, decimal Amount, string Description, GarageType? GarageType)>
        {
            // Mənzillər üçün kvadrat metr tarifi
            (RateType.PerSquareMeter, 2.50m, "Mənzillər üçün aylıq kvadrat metr tarifi", null),

            // Qarajlar üçün sabit tariflər
            (RateType.FixedGarage, 30.00m, "Açıq parkinq üçün aylıq tarif", GarageType.OpenParking),
            (RateType.FixedGarage, 50.00m, "Örtülü qaraj üçün aylıq tarif", GarageType.CoveredGarage),
            (RateType.FixedGarage, 25.00m, "Anbar üçün aylıq tarif", GarageType.Storage),
        };

        foreach (var rate in rates)
        {
            var command = new CreateRateCommand(
                rate.RateType,
                rate.Amount,
                effectiveFrom,
                rate.Description,
                rate.GarageType);

            var result = await _sender.Send(command);

            if (result.IsSuccess)
            {
                RateIds.Add(result.Value);

                var garageTypeInfo = rate.GarageType.HasValue
                    ? $" ({rate.GarageType.Value})"
                    : "";

                _logger.LogInformation("  ✅ Tarif yaradıldı: {Type}{GarageType} - {Amount} AZN",
                    rate.RateType, garageTypeInfo, rate.Amount);
            }
            else
            {
                _logger.LogError("  ❌ Tarif yaradıla bilmədi: {Error}", result.Error);
            }
        }

        _logger.LogInformation("  📊 Cəmi {Count} tarif yaradıldı", RateIds.Count);
    }
}