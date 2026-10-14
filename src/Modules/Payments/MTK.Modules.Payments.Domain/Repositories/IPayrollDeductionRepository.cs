using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Payroll;

namespace MTK.Modules.Payments.Domain.Repositories;

public interface IPayrollDeductionRepository : IRepository<PayrollDeduction>
{
    /// <summary>Verilmiş tarixdə qüvvədə olan (StartDate keçmiş, silinməmiş) bütün tutmalar.</summary>
    Task<List<PayrollDeduction>> GetActiveAsync(DateTimeOffset asOfDate, CancellationToken cancellationToken = default);
}
