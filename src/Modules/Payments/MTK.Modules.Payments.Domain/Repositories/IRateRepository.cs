using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.PropertyOwnerships;
using MTK.Modules.Payments.Domain.Rates;

namespace MTK.Modules.Payments.Domain.Repositories;

public interface IRateRepository : IRepository<Rate>
{
    Task<Rate?> GetCurrentRateAsync(RateType rateType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resolves the current FixedGarage rate for a specific garage type, falling back
    /// to the type-less default rate (GarageType == null) when no type-specific one exists.
    /// </summary>
    Task<Rate?> GetCurrentGarageRateAsync(GarageType? garageType, CancellationToken cancellationToken = default);
    Task<IEnumerable<Rate>> GetActiveRatesAsync(CancellationToken cancellationToken = default);
}
