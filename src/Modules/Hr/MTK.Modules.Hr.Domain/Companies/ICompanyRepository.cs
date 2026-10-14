using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.Companies;

public interface ICompanyRepository : IRepository<Company>
{
    /// <summary>Tək sətir — Payments-dən hələ sinxronlaşmayıbsa null.</summary>
    Task<Company?> GetAsync(CancellationToken cancellationToken = default);
}
