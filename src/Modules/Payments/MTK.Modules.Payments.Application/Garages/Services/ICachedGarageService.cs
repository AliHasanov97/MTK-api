namespace MTK.Modules.Payments.Application.Garages.Services;

public interface ICachedGarageService
{
    Task<GarageResponse?> GetByIdAsync(
        Guid garageId,
        CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, GarageResponse>> GetByIdsAsync(
        IReadOnlyCollection<Guid> garageIds,
        CancellationToken cancellationToken = default);

    Task InvalidateCacheAsync(Guid garageId, CancellationToken cancellationToken = default);
}
