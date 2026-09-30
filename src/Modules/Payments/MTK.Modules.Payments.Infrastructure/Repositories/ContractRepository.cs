using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.Contracts;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

internal sealed class ContractRepository : SearchableRepository<Contract>, IContractRepository
{
    private PaymentsDbContext PaymentsContext => (PaymentsDbContext)Context;

    public ContractRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }

    public Task<Contract?> GetWithServicesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return PaymentsContext.Contracts
            .Include(c => c.Services)
            .Include(c => c.GoodsItems)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public Task<List<Contract>> ListActiveWithServicesAsync(CancellationToken cancellationToken = default)
    {
        return PaymentsContext.Contracts
            .Include(c => c.Services)
            .Where(c => c.Status == ContractStatus.Active)
            .OrderBy(c => c.EndDate)
            .ToListAsync(cancellationToken);
    }
}
