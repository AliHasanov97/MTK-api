namespace MTK.Modules.Payments.Application.Vendors.Services;

public interface ICachedVendorService
{
    Task<VendorResponse?> GetByIdAsync(
        Guid vendorId,
        CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, VendorResponse>> GetByIdsAsync(
        IReadOnlyCollection<Guid> vendorIds,
        CancellationToken cancellationToken = default);

    Task InvalidateCacheAsync(Guid vendorId, CancellationToken cancellationToken = default);
}
