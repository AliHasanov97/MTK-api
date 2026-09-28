using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.OwnerBalances;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

internal sealed class OwnerBalanceRepository : SearchableRepository<OwnerBalance>, IOwnerBalanceRepository
{
    private PaymentsDbContext PaymentsContext => (PaymentsDbContext)Context;

    public OwnerBalanceRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<OwnerBalance?> GetByOwnerIdAsync(
        Guid ownerId,
        CancellationToken cancellationToken = default)
    {
        return await PaymentsContext.OwnerBalances
            .FirstOrDefaultAsync(ob => ob.OwnerId == ownerId, cancellationToken);
    }
}
