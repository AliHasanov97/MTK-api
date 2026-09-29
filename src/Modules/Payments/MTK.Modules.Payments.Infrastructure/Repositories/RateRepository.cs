using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.PropertyOwnerships;
using MTK.Modules.Payments.Domain.Rates;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

internal sealed class RateRepository : SearchableRepository<Rate>, IRateRepository
{
    private PaymentsDbContext PaymentsContext => (PaymentsDbContext)Context;

    public RateRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Rate?> GetCurrentRateAsync(
        RateType rateType,
        CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        return await PaymentsContext.Rates
            .Where(r => r.RateType == rateType)
            .Where(r => r.EffectiveFrom <= now && (r.EffectiveTo == null || r.EffectiveTo > now))
            .OrderByDescending(r => r.EffectiveFrom)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Rate?> GetCurrentGarageRateAsync(
        GarageType? garageType,
        CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;

        IQueryable<Rate> CurrentFixedGarageRates() => PaymentsContext.Rates
            .Where(r => r.RateType == RateType.FixedGarage)
            .Where(r => r.EffectiveFrom <= now && (r.EffectiveTo == null || r.EffectiveTo > now));

        if (garageType is not null)
        {
            Rate? specific = await CurrentFixedGarageRates()
                .Where(r => r.GarageType == garageType)
                .OrderByDescending(r => r.EffectiveFrom)
                .FirstOrDefaultAsync(cancellationToken);

            if (specific is not null)
            {
                return specific;
            }
        }

        // Fallback: the type-less default rate applies to any garage type without its own rate.
        return await CurrentFixedGarageRates()
            .Where(r => r.GarageType == null)
            .OrderByDescending(r => r.EffectiveFrom)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Rate>> GetActiveRatesAsync(
        CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        return await PaymentsContext.Rates
            .Where(r => r.EffectiveFrom <= now && (r.EffectiveTo == null || r.EffectiveTo > now))
            .ToListAsync(cancellationToken);
    }
}
