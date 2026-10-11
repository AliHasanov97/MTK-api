using AutoMapper;
using Microsoft.Extensions.Logging;
using MTK.Common.Application.Caching;
using MTK.Modules.Payments.Application.PropertyOwnerships;
using MTK.Modules.Payments.Application.PropertyOwnerships.Services;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.PropertyOwnerships;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Infrastructure.Services;

internal sealed class CachedPropertyOwnershipService(
    IPropertyOwnershipRepository propertyOwnershipRepository,
    ICacheService cacheService,
    IMapper mapper,
    ILogger<CachedPropertyOwnershipService> logger) : ICachedPropertyOwnershipService
{
    private const string PropertyOwnershipCacheKeyPrefix = "propertyownership";
    private static readonly TimeSpan PropertyOwnershipCacheDuration = TimeSpan.FromMinutes(10);

    public async Task<List<PropertyOwnershipResponse>> GetByPropertyTypeAsync(
        PropertyType propertyType,
        CancellationToken cancellationToken = default)
    {
        string cacheKey = GetPropertyOwnershipCacheKey(propertyType);

        // Try cache first
        List<PropertyOwnershipResponse>? cachedOwnerships =
            await cacheService.GetAsync<List<PropertyOwnershipResponse>>(cacheKey, cancellationToken);

        if (cachedOwnerships is not null)
        {
            logger.LogDebug("Property ownerships loaded from cache: {PropertyType}, count: {Count}",
                propertyType, cachedOwnerships.Count);
            return cachedOwnerships;
        }

        // Cache miss - load from database
        logger.LogDebug("Property ownerships not in cache, loading from database: {PropertyType}", propertyType);

        List<PropertyOwnership> ownerships = await propertyOwnershipRepository
            .GetByPropertyTypeAsync(propertyType, cancellationToken);

        // Map to response DTOs
        List<PropertyOwnershipResponse> ownershipResponses = mapper.Map<List<PropertyOwnershipResponse>>(ownerships);

        // Cache for 10 minutes
        await cacheService.SetAsync(cacheKey, ownershipResponses, PropertyOwnershipCacheDuration, cancellationToken);

        logger.LogDebug("Property ownerships cached: {PropertyType}, count: {Count} (TTL: {Duration})",
            propertyType, ownershipResponses.Count, PropertyOwnershipCacheDuration);

        return ownershipResponses;
    }

    public async Task InvalidateCacheAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Invalidating all property ownership caches");

        // Invalidate both property type caches
        await cacheService.RemoveAsync(GetPropertyOwnershipCacheKey(PropertyType.Apartment), cancellationToken);
        await cacheService.RemoveAsync(GetPropertyOwnershipCacheKey(PropertyType.Garage), cancellationToken);

        logger.LogInformation("Property ownership cache invalidation completed");
    }

    private static string GetPropertyOwnershipCacheKey(PropertyType propertyType) =>
        $"{PropertyOwnershipCacheKeyPrefix}:{propertyType}";
}
