using MediatR;
using Microsoft.Extensions.Logging;
using MTK.Modules.Payments.Application.Charges.Commands.GenerateChargesForPeriod;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.DataSeeder.Seeders;

/// <summary>
/// Sakin və vendor borclarını (charges) seed edir
/// </summary>
public sealed class ChargeSeeder
{
    private readonly ISender _sender;
    private readonly ILogger<ChargeSeeder> _logger;
    private readonly IChargeRepository _chargeRepository;

    public ChargeSeeder(ISender sender, ILogger<ChargeSeeder> logger, IChargeRepository chargeRepository)
    {
        _sender = sender;
        _logger = logger;
        _chargeRepository = chargeRepository;
    }

    public async Task SeedAsync()
    {
        // Əvvəlcə mövcud chargeları yoxla
        var existingCharges = await _chargeRepository.ListAsync();
        if (existingCharges.Any())
        {
            _logger.LogInformation("  ℹ️  Bazada artıq {Count} borc qeydi mövcuddur", existingCharges.Count());
            return;
        }

        // Son 3 ay üçün chargelar yaradaq
        var currentDate = DateTimeOffset.UtcNow;
        var periods = new List<string>();

        for (int i = 2; i >= 0; i--)
        {
            var periodDate = currentDate.AddMonths(-i);
            var period = periodDate.ToString("yyyy-MM");
            periods.Add(period);
        }

        int totalResidentCharges = 0;
        int totalVendorCharges = 0;

        foreach (var period in periods)
        {
            _logger.LogInformation("  ⏳ {Period} dövrü üçün borclar yaradılır...", period);

            var command = new GenerateChargesForPeriodCommand(period);
            var result = await _sender.Send(command);

            if (result.IsSuccess)
            {
                totalResidentCharges += result.Value.ResidentChargesCreated;
                totalVendorCharges += result.Value.VendorChargesCreated;

                _logger.LogInformation("  ✅ {Period}: {ResidentCount} sakin borcu, {VendorCount} vendor borcu",
                    period,
                    result.Value.ResidentChargesCreated,
                    result.Value.VendorChargesCreated);

                if (result.Value.ApartmentsSkippedNoRate > 0 || result.Value.GaragesSkippedNoRate > 0)
                {
                    _logger.LogWarning("  ⚠️  Tarifsiz buraxıldı: {Apartments} mənzil, {Garages} qaraj",
                        result.Value.ApartmentsSkippedNoRate,
                        result.Value.GaragesSkippedNoRate);
                }
            }
            else
            {
                _logger.LogError("  ❌ {Period} üçün borclar yaradıla bilmədi: {Error}", period, result.Error);
            }
        }

        _logger.LogInformation("  📊 Cəmi: {ResidentCount} sakin borcu, {VendorCount} vendor borcu yaradıldı",
            totalResidentCharges, totalVendorCharges);
    }
}