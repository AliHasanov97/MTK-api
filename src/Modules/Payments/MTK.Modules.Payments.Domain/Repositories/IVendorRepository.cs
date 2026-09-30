using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Vendors;

namespace MTK.Modules.Payments.Domain.Repositories;

public interface IVendorRepository : IRepository<Vendor>
{
    Task<List<Vendor>> ListActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>VÖEN unikallığı (boş VÖEN-lər yoxlanılmır).</summary>
    Task<bool> IsVoenUniqueAsync(string voen, Guid? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>Vendor-un aktiv müqaviləsi varmı — silmədən əvvəl yoxlanılır.</summary>
    Task<bool> HasActiveContractsAsync(Guid vendorId, CancellationToken cancellationToken = default);
}
