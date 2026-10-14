using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.Companies;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

internal sealed class CompanyRepository : Repository<Company>, ICompanyRepository
{
    private PaymentsDbContext PaymentsContext => (PaymentsDbContext)Context;

    public CompanyRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Company?> GetAsync(CancellationToken cancellationToken = default)
    {
        return await PaymentsContext.Companies.FirstOrDefaultAsync(cancellationToken);
    }
}
