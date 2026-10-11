namespace MTK.Common.Application.Caching;

/// <summary>
/// Distributed cache service for Redis integration.
/// Provides get/set/remove operations with TTL support.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Get value from cache by key.
    /// </summary>
    /// <typeparam name="T">Type to deserialize</typeparam>
    /// <param name="key">Cache key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cached value or null if not found</returns>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set value in cache with optional expiration.
    /// </summary>
    /// <typeparam name="T">Type to serialize</typeparam>
    /// <param name="key">Cache key</param>
    /// <param name="value">Value to cache</param>
    /// <param name="expiration">Optional expiration time (default: 5 minutes)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove value from cache by key.
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}
