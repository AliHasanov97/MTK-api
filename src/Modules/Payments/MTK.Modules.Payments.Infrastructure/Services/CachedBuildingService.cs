using AutoMapper;
using Microsoft.Extensions.Logging;
using MTK.Common.Application.Caching;
using MTK.Modules.Payments.Application.Buildings;
using MTK.Modules.Payments.Application.Buildings.Services;
using MTK.Modules.Payments.Domain.Buildings;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Infrastructure.Services;

internal sealed class CachedBuildingService : ICachedBuildingService
{
    private readonly IBuildingRepository _buildingRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;
    private readonly ILogger<CachedBuildingService> _logger;

    public CachedBuildingService(
        IBuildingRepository buildingRepository,
        ICacheService cacheService,
        IMapper mapper,
        ILogger<CachedBuildingService> logger)
    {
        _buildingRepository = buildingRepository;
        _cacheService = cacheService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<BuildingResponse?> GetByIdAsync(
        Guid buildingId,
        CancellationToken cancellationToken = default)
    {
        string cacheKey = GetBuildingCacheKey(buildingId);

        BuildingResponse? cachedBuilding = await _cacheService.GetAsync<BuildingResponse>(cacheKey, cancellationToken);
        if (cachedBuilding is not null)
        {
            _logger.LogDebug("Building loaded from cache: {BuildingId} = {Name}", buildingId, cachedBuilding.Name);
            return cachedBuilding;
        }

        Building? building = await _buildingRepository.GetByIdDefaultAsync(buildingId, cancellationToken);
        if (building is null)
        {
            _logger.LogWarning("Building not found: {BuildingId}", buildingId);
            return null;
        }

        BuildingResponse buildingResponse = _mapper.Map<BuildingResponse>(building);
        await _cacheService.SetAsync(cacheKey, buildingResponse, TimeSpan.FromHours(24), cancellationToken);

        _logger.LogDebug("Building loaded from database and cached: {BuildingId} = {Name}", buildingId, buildingResponse.Name);
        return buildingResponse;
    }

    public async Task<Dictionary<Guid, BuildingResponse>> GetByIdsAsync(
        IReadOnlyCollection<Guid> buildingIds,
        CancellationToken cancellationToken = default)
    {
        if (buildingIds.Count == 0)
        {
            return new Dictionary<Guid, BuildingResponse>();
        }

        var result = new Dictionary<Guid, BuildingResponse>();
        var uncachedIds = new List<Guid>();

        // İlk olaraq cache-dən yükləyirik
        foreach (var buildingId in buildingIds)
        {
            string cacheKey = GetBuildingCacheKey(buildingId);
            BuildingResponse? cachedBuilding = await _cacheService.GetAsync<BuildingResponse>(cacheKey, cancellationToken);

            if (cachedBuilding is not null)
            {
                result[buildingId] = cachedBuilding;
            }
            else
            {
                uncachedIds.Add(buildingId);
            }
        }

        if (uncachedIds.Count > 0)
        {
            _logger.LogDebug("Loading {Count} buildings from database (cache miss)", uncachedIds.Count);

            // Cache-də olmayan building-ləri DB-dən yükləyirik (batch)
            List<Building> buildings = await _buildingRepository.ListFromIdsAsync(uncachedIds, cancellationToken);

            foreach (var building in buildings)
            {
                BuildingResponse buildingResponse = _mapper.Map<BuildingResponse>(building);
                result[building.Id] = buildingResponse;

                // Cache-ə yazırıq (24 saat TTL - building çox az dəyişir)
                string cacheKey = GetBuildingCacheKey(building.Id);
                await _cacheService.SetAsync(cacheKey, buildingResponse, TimeSpan.FromHours(24), cancellationToken);
            }

            _logger.LogDebug("Loaded and cached {Count} buildings from database", buildings.Count);
        }
        else
        {
            _logger.LogDebug("All {Count} buildings loaded from cache (100% hit rate)", buildingIds.Count);
        }

        return result;
    }

    public async Task InvalidateCacheAsync(Guid buildingId, CancellationToken cancellationToken = default)
    {
        string cacheKey = GetBuildingCacheKey(buildingId);
        await _cacheService.RemoveAsync(cacheKey, cancellationToken);

        _logger.LogInformation("Building cache invalidated: {BuildingId}", buildingId);
    }

    private static string GetBuildingCacheKey(Guid buildingId) => $"building:{buildingId}";
}
