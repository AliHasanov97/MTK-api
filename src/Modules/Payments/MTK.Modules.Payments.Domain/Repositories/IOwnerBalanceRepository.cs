using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.OwnerBalances;

namespace MTK.Modules.Payments.Domain.Repositories;

public interface IOwnerBalanceRepository : IRepository<OwnerBalance>
{
    Task<OwnerBalance?> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);

    /// <summary>Sahiblərin balanslarını bir sorğuda gətirir (toplu yenidən hesablama üçün).</summary>
    Task<Dictionary<Guid, OwnerBalance>> GetByOwnerIdsAsync(
        IReadOnlyCollection<Guid> ownerIds, CancellationToken cancellationToken = default);
}
