using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Companies;

namespace MTK.Modules.Payments.Domain.Repositories;

public interface ICompanyRepository : IRepository<Company>
{
    /// <summary>Tək sətir — hələ yaradılmayıbsa null.</summary>
    Task<Company?> GetAsync(CancellationToken cancellationToken = default);
}
