using AutoMapper;
using Microsoft.Extensions.Logging;
using MTK.Common.Application.Caching;
using MTK.Modules.Payments.Application.Rates.Queries.GetCurrentRates;
using MTK.Modules.Payments.Application.Rates.Services;
using MTK.Modules.Payments.Domain.PropertyOwnerships;
using MTK.Modules.Payments.Domain.Rates;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Infrastructure.Services;

internal sealed class CachedRateService(
    IRateRepository rateRepository,
    ICacheService cacheService,
    IMapper mapper,
    ILogger<CachedRateService> logger) : ICachedRateService
{
    private const string RateCacheKeyPrefix = "rate";
    private static readonly TimeSpan RateCacheDuration = TimeSpan.FromHours(1);
    private static readonly TimeSpan ActiveRatesCacheDuration = TimeSpan.FromMinutes(30);

    public async Task<RateResponse?> GetCurrentRateAsync(
        RateType rateType,
        CancellationToken cancellationToken = default)
    {
        string cacheKey = GetRateCacheKey(rateType);

        // Try cache first
        RateResponse? cachedRate = await cacheService.GetAsync<RateResponse>(cacheKey, cancellationToken);
        if (cachedRate is not null)
        {
            logger.LogDebug("Rate loaded from cache: {RateType} = {Amount} AZN", rateType, cachedRate.Amount);
            return cachedRate;
        }

        // Cache miss - load from database
        logger.LogDebug("Rate not in cache, loading from database: {RateType}", rateType);
        Rate? rate = await rateRepository.GetCurrentRateAsync(rateType, cancellationToken);

        if (rate is null)
        {
            logger.LogWarning("No active rate found for type {RateType}", rateType);
            return null;
        }

        // Map to response DTO
        RateResponse rateResponse = mapper.Map<RateResponse>(rate);

        // Cache for 1 hour
        await cacheService.SetAsync(cacheKey, rateResponse, RateCacheDuration, cancellationToken);
        logger.LogDebug("Rate cached: {RateType} = {Amount} AZN (TTL: {Duration})",
            rateType, rateResponse.Amount, RateCacheDuration);

        return rateResponse;
    }

    public async Task<RateResponse?> GetCurrentGarageRateAsync(
        GarageType? garageType,
        CancellationToken cancellationToken = default)
    {
        string cacheKey = GetGarageRateCacheKey(garageType);

        // Try cache first
        RateResponse? cachedRate = await cacheService.GetAsync<RateResponse>(cacheKey, cancellationToken);
        if (cachedRate is not null)
        {
            logger.LogDebug("Garage rate loaded from cache: {GarageType} = {Amount} AZN",
                garageType?.ToString() ?? "Default", cachedRate.Amount);
            return cachedRate;
        }

        // Cache miss - load from database
        logger.LogDebug("Garage rate not in cache, loading from database: {GarageType}",
            garageType?.ToString() ?? "Default");
        Rate? rate = await rateRepository.GetCurrentGarageRateAsync(garageType, cancellationToken);

        if (rate is null)
        {
            logger.LogWarning("No active garage rate found for type {GarageType}",
                garageType?.ToString() ?? "Default");
            return null;
        }

        // Map to response DTO
        RateResponse rateResponse = mapper.Map<RateResponse>(rate);

        // Cache for 1 hour
        await cacheService.SetAsync(cacheKey, rateResponse, RateCacheDuration, cancellationToken);
        logger.LogDebug("Garage rate cached: {GarageType} = {Amount} AZN (TTL: {Duration})",
            garageType?.ToString() ?? "Default", rateResponse.Amount, RateCacheDuration);

        return rateResponse;
    }

    public async Task<IReadOnlyCollection<RateResponse>> GetActiveRatesAsync(
        CancellationToken cancellationToken = default)
    {
        const string cacheKey = $"{RateCacheKeyPrefix}:active:all";

        // Try cache first
        List<RateResponse>? cachedRates = await cacheService.GetAsync<List<RateResponse>>(cacheKey, cancellationToken);
        if (cachedRates is not null)
        {
            logger.LogDebug("Active rates loaded from cache: {Count} rates", cachedRates.Count);
            return cachedRates;
        }

        // Cache miss - load from database
        logger.LogDebug("Active rates not in cache, loading from database");
        IEnumerable<Rate> rates = await rateRepository.GetActiveRatesAsync(cancellationToken);
        List<RateResponse> rateResponses = mapper.Map<List<RateResponse>>(rates);

        // Cache for 30 minutes
        await cacheService.SetAsync(cacheKey, rateResponses, ActiveRatesCacheDuration, cancellationToken);
        logger.LogDebug("Active rates cached: {Count} rates (TTL: {Duration})",
            rateResponses.Count, ActiveRatesCacheDuration);

        return rateResponses;
    }

    public async Task InvalidateCacheAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Invalidating all rate caches");

        // Invalidate all rate type caches
        await cacheService.RemoveAsync(GetRateCacheKey(RateType.PerSquareMeter), cancellationToken);
        await cacheService.RemoveAsync(GetRateCacheKey(RateType.FixedGarage), cancellationToken);
        await cacheService.RemoveAsync(GetRateCacheKey(RateType.Manual), cancellationToken);

        // Invalidate all garage type caches
        await cacheService.RemoveAsync(GetGarageRateCacheKey(null), cancellationToken); // Default
        await cacheService.RemoveAsync(GetGarageRateCacheKey(GarageType.OpenParking), cancellationToken);
        await cacheService.RemoveAsync(GetGarageRateCacheKey(GarageType.CoveredGarage), cancellationToken);
        await cacheService.RemoveAsync(GetGarageRateCacheKey(GarageType.Storage), cancellationToken);

        // Invalidate active rates list
        await cacheService.RemoveAsync($"{RateCacheKeyPrefix}:active:all", cancellationToken);

        logger.LogInformation("Rate cache invalidation completed");
    }

    private static string GetRateCacheKey(RateType rateType) =>
        $"{RateCacheKeyPrefix}:{rateType}";

    private static string GetGarageRateCacheKey(GarageType? garageType) =>
        $"{RateCacheKeyPrefix}:garage:{garageType?.ToString() ?? "default"}";
}
