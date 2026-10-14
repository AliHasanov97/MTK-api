using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.Payroll;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

internal sealed class PayrollBonusRepository : Repository<PayrollBonus>, IPayrollBonusRepository
{
    private PaymentsDbContext PaymentsContext => (PaymentsDbContext)Context;

    public PayrollBonusRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }

    public Task<List<PayrollBonus>> GetForPeriodAsync(int year, int month, CancellationToken cancellationToken = default)
    {
        return PaymentsContext.PayrollBonuses
            .Where(b => b.SalaryYear == year && b.SalaryMonth == month)
            .ToListAsync(cancellationToken);
    }
}
