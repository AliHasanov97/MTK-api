using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.OwnerBalances;

namespace MTK.Modules.Payments.Domain.Repositories;

public interface IOwnerBalanceRepository : IRepository<OwnerBalance>
{
    Task<OwnerBalance?> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
}
