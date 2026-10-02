using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.CompanyBalances;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

internal sealed class CompanyBalanceRepository : Repository<CompanyBalance>, ICompanyBalanceRepository
{
    private PaymentsDbContext PaymentsContext => (PaymentsDbContext)Context;

    public CompanyBalanceRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<CompanyBalance?> GetAsync(CancellationToken cancellationToken = default)
    {
        return await PaymentsContext.CompanyBalances.FirstOrDefaultAsync(cancellationToken);
    }
}
