using MTK.Modules.Payments.Application.Rates.Queries.GetCurrentRates;
using MTK.Modules.Payments.Domain.PropertyOwnerships;
using MTK.Modules.Payments.Domain.Rates;

namespace MTK.Modules.Payments.Application.Rates.Services;

/// <summary>
/// Redis-cached service for rate queries. Returns response DTOs instead of domain entities.
/// Cache invalidation happens automatically on Create/Update/Deactivate operations.
/// </summary>
public interface ICachedRateService
{
    /// <summary>
    /// Gets the current active rate for the specified rate type.
    /// Cached for 1 hour (rates change monthly at most).
    /// </summary>
    /// <param name="rateType">Type of rate (PerSquareMeter, FixedGarage, Manual)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Current rate response or null if no active rate exists</returns>
    Task<RateResponse?> GetCurrentRateAsync(
        RateType rateType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current active garage rate for the specified garage type with fallback.
    /// First tries type-specific rate, then falls back to default (null GarageType) rate.
    /// Cached for 1 hour per garage type.
    /// </summary>
    /// <param name="garageType">Garage type (OpenParking, CoveredGarage, Storage) or null for default</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Current garage rate response or null if no active rate exists</returns>
    Task<RateResponse?> GetCurrentGarageRateAsync(
        GarageType? garageType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all currently active rates (all types).
    /// Cached for 30 minutes (used by UI to display current rates).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of all active rates</returns>
    Task<IReadOnlyCollection<RateResponse>> GetActiveRatesAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates all rate-related cache entries.
    /// Call this after Create/Update/Deactivate operations.
    /// </summary>
    Task InvalidateCacheAsync(CancellationToken cancellationToken = default);
}
