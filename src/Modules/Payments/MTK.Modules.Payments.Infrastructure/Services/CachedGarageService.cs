using AutoMapper;
using Microsoft.Extensions.Logging;
using MTK.Common.Application.Caching;
using MTK.Modules.Payments.Application.Garages;
using MTK.Modules.Payments.Application.Garages.Services;
using MTK.Modules.Payments.Domain.Garages;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Infrastructure.Services;

internal sealed class CachedGarageService : ICachedGarageService
{
    private readonly IGarageRepository _garageRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;
    private readonly ILogger<CachedGarageService> _logger;

    public CachedGarageService(
        IGarageRepository garageRepository,
        ICacheService cacheService,
        IMapper mapper,
        ILogger<CachedGarageService> logger)
    {
        _garageRepository = garageRepository;
        _cacheService = cacheService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GarageResponse?> GetByIdAsync(
        Guid garageId,
        CancellationToken cancellationToken = default)
    {
        string cacheKey = GetGarageCacheKey(garageId);

        GarageResponse? cachedGarage = await _cacheService.GetAsync<GarageResponse>(cacheKey, cancellationToken);
        if (cachedGarage is not null)
        {
            _logger.LogDebug("Garage loaded from cache: {GarageId} = {Number}", garageId, cachedGarage.GarageNumber);
            return cachedGarage;
        }

        Garage? garage = await _garageRepository.GetByIdDefaultAsync(garageId, cancellationToken);
        if (garage is null)
        {
            _logger.LogWarning("Garage not found: {GarageId}", garageId);
            return null;
        }

        GarageResponse garageResponse = _mapper.Map<GarageResponse>(garage);
        await _cacheService.SetAsync(cacheKey, garageResponse, TimeSpan.FromHours(24), cancellationToken);

        _logger.LogDebug("Garage loaded from database and cached: {GarageId} = {Number}", garageId, garageResponse.GarageNumber);
        return garageResponse;
    }

    public async Task<Dictionary<Guid, GarageResponse>> GetByIdsAsync(
        IReadOnlyCollection<Guid> garageIds,
        CancellationToken cancellationToken = default)
    {
        if (garageIds.Count == 0)
        {
            return new Dictionary<Guid, GarageResponse>();
        }

        var result = new Dictionary<Guid, GarageResponse>();
        var uncachedIds = new List<Guid>();

        // İlk olaraq cache-dən yükləyirik
        foreach (var garageId in garageIds)
        {
            string cacheKey = GetGarageCacheKey(garageId);
            GarageResponse? cachedGarage = await _cacheService.GetAsync<GarageResponse>(cacheKey, cancellationToken);

            if (cachedGarage is not null)
            {
                result[garageId] = cachedGarage;
            }
            else
            {
                uncachedIds.Add(garageId);
            }
        }

        if (uncachedIds.Count > 0)
        {
            _logger.LogDebug("Loading {Count} garages from database (cache miss)", uncachedIds.Count);

            // Cache-də olmayan garage-ləri DB-dən yükləyirik (batch)
            List<Garage> garages = await _garageRepository.ListFromIdsAsync(uncachedIds, cancellationToken);

            foreach (var garage in garages)
            {
                GarageResponse garageResponse = _mapper.Map<GarageResponse>(garage);
                result[garage.Id] = garageResponse;

                // Cache-ə yazırıq (24 saat TTL - garage çox az dəyişir)
                string cacheKey = GetGarageCacheKey(garage.Id);
                await _cacheService.SetAsync(cacheKey, garageResponse, TimeSpan.FromHours(24), cancellationToken);
            }

            _logger.LogDebug("Loaded and cached {Count} garages from database", garages.Count);
        }
        else
        {
            _logger.LogDebug("All {Count} garages loaded from cache (100% hit rate)", garageIds.Count);
        }

        return result;
    }

    public async Task InvalidateCacheAsync(Guid garageId, CancellationToken cancellationToken = default)
    {
        string cacheKey = GetGarageCacheKey(garageId);
        await _cacheService.RemoveAsync(cacheKey, cancellationToken);

        _logger.LogInformation("Garage cache invalidated: {GarageId}", garageId);
    }

    private static string GetGarageCacheKey(Guid garageId) => $"garage:{garageId}";
}
