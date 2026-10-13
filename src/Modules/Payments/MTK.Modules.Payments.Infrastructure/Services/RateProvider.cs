using Microsoft.EntityFrameworkCore;
using MTK.Modules.Payments.Application.Rates.Queries.GetCurrentRates;
using MTK.Modules.Payments.Domain.PropertyOwnerships;
using MTK.Modules.Payments.Domain.Rates;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Services;

/// <summary>
/// Tarixi tarifləri təmin edən servis (retroaktiv borclar üçün)
/// </summary>
internal sealed class RateProvider(PaymentsDbContext dbContext)
{
    /// <summary>
    /// Müəyyən tarixdə aktiv olan tarifi alır
    /// </summary>
    public async Task<RateResponse?> GetRateAtDateAsync(
        RateType rateType,
        DateTimeOffset asOfDate,
        CancellationToken cancellationToken)
    {
        Rate? rate = await dbContext.Rates
            .Where(r => r.RateType == rateType)
            .Where(r => r.EffectiveFrom <= asOfDate && (r.EffectiveTo == null || r.EffectiveTo > asOfDate))
            .OrderByDescending(r => r.EffectiveFrom)
            .FirstOrDefaultAsync(cancellationToken);

        return rate is not null
            ? new RateResponse(
                rate.Id,
                rate.RateType,
                rate.Amount,
                rate.EffectiveFrom,
                rate.EffectiveTo,
                rate.Description,
                null)
            : null;
    }

    /// <summary>
    /// Müəyyən tarixdə aktiv olan qaraj tarifini alır (tip-spesifik və ya default)
    /// </summary>
    public async Task<RateResponse?> GetGarageRateAtDateAsync(
        GarageType? garageType,
        DateTimeOffset asOfDate,
        CancellationToken cancellationToken)
    {
        // Əvvəlcə tip-spesifik tarifi yoxla
        if (garageType is not null)
        {
            Rate? specificRate = await dbContext.Rates
                .Where(r => r.RateType == RateType.FixedGarage && r.GarageType == garageType)
                .Where(r => r.EffectiveFrom <= asOfDate && (r.EffectiveTo == null || r.EffectiveTo > asOfDate))
                .OrderByDescending(r => r.EffectiveFrom)
                .FirstOrDefaultAsync(cancellationToken);

            if (specificRate is not null)
            {
                return new RateResponse(
                    specificRate.Id,
                    specificRate.RateType,
                    specificRate.Amount,
                    specificRate.EffectiveFrom,
                    specificRate.EffectiveTo,
                    specificRate.Description,
                    specificRate.GarageType);
            }
        }

        // Fallback: Default tarif (GarageType == null)
        Rate? defaultRate = await dbContext.Rates
            .Where(r => r.RateType == RateType.FixedGarage && r.GarageType == null)
            .Where(r => r.EffectiveFrom <= asOfDate && (r.EffectiveTo == null || r.EffectiveTo > asOfDate))
            .OrderByDescending(r => r.EffectiveFrom)
            .FirstOrDefaultAsync(cancellationToken);

        return defaultRate is not null
            ? new RateResponse(
                defaultRate.Id,
                defaultRate.RateType,
                defaultRate.Amount,
                defaultRate.EffectiveFrom,
                defaultRate.EffectiveTo,
                defaultRate.Description,
                defaultRate.GarageType)
            : null;
    }
}
