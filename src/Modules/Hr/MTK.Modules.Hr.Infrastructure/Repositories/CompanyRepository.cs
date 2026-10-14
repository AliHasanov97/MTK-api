using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.Companies;
using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class CompanyRepository : Repository<Company>, ICompanyRepository
{
    public CompanyRepository(HrDbContext context) : base(context) { }

    public async Task<Company?> GetAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<Company>().FirstOrDefaultAsync(cancellationToken);
    }
}
