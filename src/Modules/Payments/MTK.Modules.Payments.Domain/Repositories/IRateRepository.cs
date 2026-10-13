using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.PropertyOwnerships;
using MTK.Modules.Payments.Domain.Rates;

namespace MTK.Modules.Payments.Domain.Repositories;

public interface IRateRepository : IRepository<Rate>
{
    Task<Rate?> GetCurrentRateAsync(RateType rateType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Müəyyən bir tarixdə aktiv olan tarifi alır (retroaktiv borc yaratmaq üçün).
    /// </summary>
    /// <param name="rateType">Tarif növü</param>
    /// <param name="asOf">Hansı tarixdə tarifdən istifadə olunacaq (period-un ilk günü)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Həmin tarixdə aktiv olan tarif və ya null</returns>
    Task<Rate?> GetRateAtDateAsync(RateType rateType, DateTimeOffset asOf, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resolves the current FixedGarage rate for a specific garage type, falling back
    /// to the type-less default rate (GarageType == null) when no type-specific one exists.
    /// </summary>
    Task<Rate?> GetCurrentGarageRateAsync(GarageType? garageType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Müəyyən bir tarixdə aktiv olan qaraj tarifini alır (retroaktiv borc yaratmaq üçün).
    /// Əvvəlcə tip-spesifik tarifi axtarır, sonra default-a (null GarageType) fallback edir.
    /// </summary>
    Task<Rate?> GetGarageRateAtDateAsync(GarageType? garageType, DateTimeOffset asOf, CancellationToken cancellationToken = default);

    Task<IEnumerable<Rate>> GetActiveRatesAsync(CancellationToken cancellationToken = default);
}
