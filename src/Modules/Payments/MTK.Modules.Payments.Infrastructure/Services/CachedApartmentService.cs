using AutoMapper;
using Microsoft.Extensions.Logging;
using MTK.Common.Application.Caching;
using MTK.Modules.Payments.Application.Apartments;
using MTK.Modules.Payments.Application.Apartments.Services;
using MTK.Modules.Payments.Domain.Apartments;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Infrastructure.Services;

internal sealed class CachedApartmentService : ICachedApartmentService
{
    private readonly IApartmentRepository _apartmentRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;
    private readonly ILogger<CachedApartmentService> _logger;

    public CachedApartmentService(
        IApartmentRepository apartmentRepository,
        ICacheService cacheService,
        IMapper mapper,
        ILogger<CachedApartmentService> logger)
    {
        _apartmentRepository = apartmentRepository;
        _cacheService = cacheService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApartmentResponse?> GetByIdAsync(
        Guid apartmentId,
        CancellationToken cancellationToken = default)
    {
        string cacheKey = GetApartmentCacheKey(apartmentId);

        ApartmentResponse? cachedApartment = await _cacheService.GetAsync<ApartmentResponse>(cacheKey, cancellationToken);
        if (cachedApartment is not null)
        {
            _logger.LogDebug("Apartment loaded from cache: {ApartmentId} = {Number}", apartmentId, cachedApartment.ApartmentNumber);
            return cachedApartment;
        }

        Apartment? apartment = await _apartmentRepository.GetByIdDefaultAsync(apartmentId, cancellationToken);
        if (apartment is null)
        {
            _logger.LogWarning("Apartment not found: {ApartmentId}", apartmentId);
            return null;
        }

        ApartmentResponse apartmentResponse = _mapper.Map<ApartmentResponse>(apartment);
        await _cacheService.SetAsync(cacheKey, apartmentResponse, TimeSpan.FromHours(24), cancellationToken);

        _logger.LogDebug("Apartment loaded from database and cached: {ApartmentId} = {Number}", apartmentId, apartmentResponse.ApartmentNumber);
        return apartmentResponse;
    }

    public async Task<Dictionary<Guid, ApartmentResponse>> GetByIdsAsync(
        IReadOnlyCollection<Guid> apartmentIds,
        CancellationToken cancellationToken = default)
    {
        if (apartmentIds.Count == 0)
        {
            return new Dictionary<Guid, ApartmentResponse>();
        }

        var result = new Dictionary<Guid, ApartmentResponse>();
        var uncachedIds = new List<Guid>();

        // İlk olaraq cache-dən yükləyirik
        foreach (var apartmentId in apartmentIds)
        {
            string cacheKey = GetApartmentCacheKey(apartmentId);
            ApartmentResponse? cachedApartment = await _cacheService.GetAsync<ApartmentResponse>(cacheKey, cancellationToken);

            if (cachedApartment is not null)
            {
                result[apartmentId] = cachedApartment;
            }
            else
            {
                uncachedIds.Add(apartmentId);
            }
        }

        if (uncachedIds.Count > 0)
        {
            _logger.LogDebug("Loading {Count} apartments from database (cache miss)", uncachedIds.Count);

            // Cache-də olmayan apartment-ləri DB-dən yükləyirik (batch)
            List<Apartment> apartments = await _apartmentRepository.ListFromIdsAsync(uncachedIds, cancellationToken);

            foreach (var apartment in apartments)
            {
                ApartmentResponse apartmentResponse = _mapper.Map<ApartmentResponse>(apartment);
                result[apartment.Id] = apartmentResponse;

                // Cache-ə yazırıq (24 saat TTL - apartment çox az dəyişir)
                string cacheKey = GetApartmentCacheKey(apartment.Id);
                await _cacheService.SetAsync(cacheKey, apartmentResponse, TimeSpan.FromHours(24), cancellationToken);
            }

            _logger.LogDebug("Loaded and cached {Count} apartments from database", apartments.Count);
        }
        else
        {
            _logger.LogDebug("All {Count} apartments loaded from cache (100% hit rate)", apartmentIds.Count);
        }

        return result;
    }

    public async Task InvalidateCacheAsync(Guid apartmentId, CancellationToken cancellationToken = default)
    {
        string cacheKey = GetApartmentCacheKey(apartmentId);
        await _cacheService.RemoveAsync(cacheKey, cancellationToken);

        _logger.LogInformation("Apartment cache invalidated: {ApartmentId}", apartmentId);
    }

    private static string GetApartmentCacheKey(Guid apartmentId) => $"apartment:{apartmentId}";
}
