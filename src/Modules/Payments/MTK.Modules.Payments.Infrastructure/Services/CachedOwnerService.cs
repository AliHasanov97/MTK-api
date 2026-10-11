using AutoMapper;
using Microsoft.Extensions.Logging;
using MTK.Common.Application.Caching;
using MTK.Modules.Payments.Application.Owners;
using MTK.Modules.Payments.Application.Owners.Services;
using MTK.Modules.Payments.Domain.Owners;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Infrastructure.Services;

internal sealed class CachedOwnerService : ICachedOwnerService
{
    private readonly IOwnerRepository _ownerRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;
    private readonly ILogger<CachedOwnerService> _logger;

    public CachedOwnerService(
        IOwnerRepository ownerRepository,
        ICacheService cacheService,
        IMapper mapper,
        ILogger<CachedOwnerService> logger)
    {
        _ownerRepository = ownerRepository;
        _cacheService = cacheService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<OwnerResponse?> GetByIdAsync(
        Guid ownerId,
        CancellationToken cancellationToken = default)
    {
        string cacheKey = GetOwnerCacheKey(ownerId);

        OwnerResponse? cachedOwner = await _cacheService.GetAsync<OwnerResponse>(cacheKey, cancellationToken);
        if (cachedOwner is not null)
        {
            _logger.LogDebug("Owner loaded from cache: {OwnerId} = {FullName}", ownerId, cachedOwner.FullName);
            return cachedOwner;
        }

        Owner? owner = await _ownerRepository.GetByIdDefaultAsync(ownerId, cancellationToken);
        if (owner is null)
        {
            _logger.LogWarning("Owner not found: {OwnerId}", ownerId);
            return null;
        }

        OwnerResponse ownerResponse = _mapper.Map<OwnerResponse>(owner);
        await _cacheService.SetAsync(cacheKey, ownerResponse, TimeSpan.FromHours(1), cancellationToken);

        _logger.LogDebug("Owner loaded from database and cached: {OwnerId} = {FullName}", ownerId, ownerResponse.FullName);
        return ownerResponse;
    }

    public async Task<Dictionary<Guid, OwnerResponse>> GetByIdsAsync(
        IReadOnlyCollection<Guid> ownerIds,
        CancellationToken cancellationToken = default)
    {
        if (ownerIds.Count == 0)
        {
            return new Dictionary<Guid, OwnerResponse>();
        }

        var result = new Dictionary<Guid, OwnerResponse>();
        var uncachedIds = new List<Guid>();

        // İlk olaraq cache-dən yükləyirik
        foreach (var ownerId in ownerIds)
        {
            string cacheKey = GetOwnerCacheKey(ownerId);
            OwnerResponse? cachedOwner = await _cacheService.GetAsync<OwnerResponse>(cacheKey, cancellationToken);

            if (cachedOwner is not null)
            {
                result[ownerId] = cachedOwner;
            }
            else
            {
                uncachedIds.Add(ownerId);
            }
        }

        if (uncachedIds.Count > 0)
        {
            _logger.LogDebug("Loading {Count} owners from database (cache miss)", uncachedIds.Count);

            // Cache-də olmayan owner-ləri DB-dən yükləyirik (batch)
            List<Owner> owners = await _ownerRepository.ListFromIdsAsync(uncachedIds, cancellationToken);

            foreach (var owner in owners)
            {
                OwnerResponse ownerResponse = _mapper.Map<OwnerResponse>(owner);
                result[owner.Id] = ownerResponse;

                // Cache-ə yazırıq
                string cacheKey = GetOwnerCacheKey(owner.Id);
                await _cacheService.SetAsync(cacheKey, ownerResponse, TimeSpan.FromHours(1), cancellationToken);
            }

            _logger.LogDebug("Loaded and cached {Count} owners from database", owners.Count);
        }
        else
        {
            _logger.LogDebug("All {Count} owners loaded from cache (100% hit rate)", ownerIds.Count);
        }

        return result;
    }

    public async Task InvalidateCacheAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        string cacheKey = GetOwnerCacheKey(ownerId);
        await _cacheService.RemoveAsync(cacheKey, cancellationToken);

        _logger.LogInformation("Owner cache invalidated: {OwnerId}", ownerId);
    }

    private static string GetOwnerCacheKey(Guid ownerId) => $"owner:{ownerId}";
}
