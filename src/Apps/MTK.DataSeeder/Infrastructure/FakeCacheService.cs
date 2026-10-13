using MTK.Common.Application.Caching;

namespace MTK.DataSeeder.Infrastructure;

/// <summary>
/// Fake CacheService for CLI/Seeder - cache-ə ehtiyac yoxdur seeder-də
/// </summary>
internal sealed class FakeCacheService : ICacheService
{
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        // CLI-də cache yoxdur, həmişə null qaytarır
        return Task.FromResult<T?>(default);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        // CLI-də cache yoxdur, heç nə etmir
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        // CLI-də cache yoxdur, heç nə etmir
        return Task.CompletedTask;
    }
}