using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Payroll;

namespace MTK.Modules.Payments.Domain.Repositories;

public interface IPayrollTaxBracketRepository : IRepository<PayrollTaxBracket>
{
    /// <summary>
    /// Verilmiş tarixdə qüvvədə olan bütün pillələr (hər <see cref="PayrollRateType"/>/
    /// <see cref="PayrollParty"/> kombinasiyası üçün) — əmək haqqı hesablaması üçün.
    /// </summary>
    Task<List<PayrollTaxBracket>> GetEffectiveBracketsAsync(DateTimeOffset asOfDate, CancellationToken cancellationToken = default);
}
