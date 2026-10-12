using AutoMapper;
using Microsoft.Extensions.Logging;
using MTK.Common.Application.Caching;
using MTK.Modules.Payments.Application.Vendors;
using MTK.Modules.Payments.Application.Vendors.Services;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Domain.Vendors;

namespace MTK.Modules.Payments.Infrastructure.Services;

internal sealed class CachedVendorService : ICachedVendorService
{
    private readonly IVendorRepository _vendorRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;
    private readonly ILogger<CachedVendorService> _logger;

    public CachedVendorService(
        IVendorRepository vendorRepository,
        ICacheService cacheService,
        IMapper mapper,
        ILogger<CachedVendorService> logger)
    {
        _vendorRepository = vendorRepository;
        _cacheService = cacheService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<VendorResponse?> GetByIdAsync(
        Guid vendorId,
        CancellationToken cancellationToken = default)
    {
        string cacheKey = GetVendorCacheKey(vendorId);

        VendorResponse? cachedVendor = await _cacheService.GetAsync<VendorResponse>(cacheKey, cancellationToken);
        if (cachedVendor is not null)
        {
            _logger.LogDebug("Vendor loaded from cache: {VendorId} = {Name}", vendorId, cachedVendor.Name);
            return cachedVendor;
        }

        Vendor? vendor = await _vendorRepository.GetByIdDefaultAsync(vendorId, cancellationToken);
        if (vendor is null)
        {
            _logger.LogWarning("Vendor not found: {VendorId}", vendorId);
            return null;
        }

        VendorResponse vendorResponse = _mapper.Map<VendorResponse>(vendor);
        await _cacheService.SetAsync(cacheKey, vendorResponse, TimeSpan.FromHours(24), cancellationToken);

        _logger.LogDebug("Vendor loaded from database and cached: {VendorId} = {Name}", vendorId, vendorResponse.Name);
        return vendorResponse;
    }

    public async Task<Dictionary<Guid, VendorResponse>> GetByIdsAsync(
        IReadOnlyCollection<Guid> vendorIds,
        CancellationToken cancellationToken = default)
    {
        if (vendorIds.Count == 0)
        {
            return new Dictionary<Guid, VendorResponse>();
        }

        var result = new Dictionary<Guid, VendorResponse>();
        var uncachedIds = new List<Guid>();

        // İlk olaraq cache-dən yükləyirik
        foreach (var vendorId in vendorIds)
        {
            string cacheKey = GetVendorCacheKey(vendorId);
            VendorResponse? cachedVendor = await _cacheService.GetAsync<VendorResponse>(cacheKey, cancellationToken);

            if (cachedVendor is not null)
            {
                result[vendorId] = cachedVendor;
            }
            else
            {
                uncachedIds.Add(vendorId);
            }
        }

        if (uncachedIds.Count > 0)
        {
            _logger.LogDebug("Loading {Count} vendors from database (cache miss)", uncachedIds.Count);

            // Cache-də olmayan vendor-ləri DB-dən yükləyirik (batch)
            List<Vendor> vendors = await _vendorRepository.ListFromIdsAsync(uncachedIds, cancellationToken);

            foreach (var vendor in vendors)
            {
                VendorResponse vendorResponse = _mapper.Map<VendorResponse>(vendor);
                result[vendor.Id] = vendorResponse;

                // Cache-ə yazırıq (24 saat TTL - vendor çox az dəyişir)
                string cacheKey = GetVendorCacheKey(vendor.Id);
                await _cacheService.SetAsync(cacheKey, vendorResponse, TimeSpan.FromHours(24), cancellationToken);
            }

            _logger.LogDebug("Loaded and cached {Count} vendors from database", vendors.Count);
        }
        else
        {
            _logger.LogDebug("All {Count} vendors loaded from cache (100% hit rate)", vendorIds.Count);
        }

        return result;
    }

    public async Task InvalidateCacheAsync(Guid vendorId, CancellationToken cancellationToken = default)
    {
        string cacheKey = GetVendorCacheKey(vendorId);
        await _cacheService.RemoveAsync(cacheKey, cancellationToken);

        _logger.LogInformation("Vendor cache invalidated: {VendorId}", vendorId);
    }

    private static string GetVendorCacheKey(Guid vendorId) => $"vendor:{vendorId}";
}
