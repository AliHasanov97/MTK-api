using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Payroll;

namespace MTK.Modules.Payments.Domain.Repositories;

public interface IPayrollRunRepository : IRepository<PayrollRun>
{
    /// <summary>Qəbzləri ilə birlikdə bir PayrollRun.</summary>
    Task<PayrollRun?> GetWithPayslipsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Verilmiş ay/il üçün mövcud PayrollRun (təkrar hesablamanın qarşısını almaq üçün).</summary>
    Task<PayrollRun?> GetByPeriodAsync(int year, int month, CancellationToken cancellationToken = default);

    /// <summary>Verilmiş ay/il üçün mövcud PayrollRun, qəbzləri ilə birlikdə (UI-da baxış üçün).</summary>
    Task<PayrollRun?> GetByPeriodWithPayslipsAsync(int year, int month, CancellationToken cancellationToken = default);
}
