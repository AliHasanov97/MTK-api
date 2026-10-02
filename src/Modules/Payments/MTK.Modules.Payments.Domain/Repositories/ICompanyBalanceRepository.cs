using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.CompanyBalances;

namespace MTK.Modules.Payments.Domain.Repositories;

public interface ICompanyBalanceRepository : IRepository<CompanyBalance>
{
    /// <summary>The one row, if it's been created yet (null before the first recalculation).</summary>
    Task<CompanyBalance?> GetAsync(CancellationToken cancellationToken = default);
}
