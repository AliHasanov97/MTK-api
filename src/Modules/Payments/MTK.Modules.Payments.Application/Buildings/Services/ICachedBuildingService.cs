namespace MTK.Modules.Payments.Application.Buildings.Services;

/// <summary>
/// Building məlumatları üçün cached service
/// Display enrichment zamanı building name/address-i Redis-dən yükləyir
/// </summary>
public interface ICachedBuildingService
{
    /// <summary>
    /// Bir building-in məlumatlarını cache-dən və ya DB-dən yükləyir
    /// </summary>
    Task<BuildingResponse?> GetByIdAsync(
        Guid buildingId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Bir neçə building-in məlumatlarını batch olaraq yükləyir (N+1 problem-dən qaçmaq üçün)
    /// </summary>
    Task<Dictionary<Guid, BuildingResponse>> GetByIdsAsync(
        IReadOnlyCollection<Guid> buildingIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Building cache-ini invalidate edir (building create/update zamanı)
    /// </summary>
    Task InvalidateCacheAsync(Guid buildingId, CancellationToken cancellationToken = default);
}
