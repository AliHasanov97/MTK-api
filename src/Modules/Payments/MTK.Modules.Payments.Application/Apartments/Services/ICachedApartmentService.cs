namespace MTK.Modules.Payments.Application.Apartments.Services;

public interface ICachedApartmentService
{
    Task<ApartmentResponse?> GetByIdAsync(
        Guid apartmentId,
        CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, ApartmentResponse>> GetByIdsAsync(
        IReadOnlyCollection<Guid> apartmentIds,
        CancellationToken cancellationToken = default);

    Task InvalidateCacheAsync(Guid apartmentId, CancellationToken cancellationToken = default);
}
