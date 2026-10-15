using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Payroll;

namespace MTK.Modules.Payments.Domain.Repositories;

public interface IPayrollAdjustmentRepository : IRepository<PayrollAdjustment>
{
    /// <summary>Verilmiş ay üçün bütün işçilərin əmək haqqı düzəlişləri (hesablama üçün, tək sorğu).</summary>
    Task<List<PayrollAdjustment>> GetForPeriodAsync(int year, int month, CancellationToken cancellationToken = default);
}
