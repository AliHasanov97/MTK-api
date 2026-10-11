namespace MTK.Modules.Payments.Application.Owners.Services;

/// <summary>
/// Owner məlumatları üçün cached service
/// Display enrichment zamanı owner adlarını Redis-dən yükləyir
/// </summary>
public interface ICachedOwnerService
{
    /// <summary>
    /// Bir owner-in məlumatlarını cache-dən və ya DB-dən yükləyir
    /// </summary>
    Task<OwnerResponse?> GetByIdAsync(
        Guid ownerId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Bir neçə owner-in məlumatlarını batch olaraq yükləyir (N+1 problem-dən qaçmaq üçün)
    /// </summary>
    Task<Dictionary<Guid, OwnerResponse>> GetByIdsAsync(
        IReadOnlyCollection<Guid> ownerIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Owner cache-ini invalidate edir (owner create/update zamanı)
    /// </summary>
    Task InvalidateCacheAsync(Guid ownerId, CancellationToken cancellationToken = default);
}
