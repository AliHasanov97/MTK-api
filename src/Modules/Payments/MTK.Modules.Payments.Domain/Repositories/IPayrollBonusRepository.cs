using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Payroll;

namespace MTK.Modules.Payments.Domain.Repositories;

public interface IPayrollBonusRepository : IRepository<PayrollBonus>
{
    /// <summary>Verilmiş ay üçün bütün işçilərin bonusları (əmək haqqı hesablaması üçün, tək sorğu).</summary>
    Task<List<PayrollBonus>> GetForPeriodAsync(int year, int month, CancellationToken cancellationToken = default);
}
