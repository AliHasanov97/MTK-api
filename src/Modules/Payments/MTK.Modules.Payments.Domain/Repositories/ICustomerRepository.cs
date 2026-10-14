using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Customers;

namespace MTK.Modules.Payments.Domain.Repositories;

public interface ICustomerRepository : IRepository<Customer>
{
    /// <summary>VÖEN unikallığı (boş VÖEN-lər yoxlanılmır).</summary>
    Task<bool> IsVoenUniqueAsync(string voen, Guid? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>Müştərinin aktiv müqaviləsi və ya açıq borcu varmı — silmədən əvvəl yoxlanılır.</summary>
    Task<bool> HasActiveContractsAsync(Guid customerId, CancellationToken cancellationToken = default);

    Task<bool> HasOpenChargesAsync(Guid customerId, CancellationToken cancellationToken = default);
}
