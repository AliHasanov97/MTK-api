using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.Payroll;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

internal sealed class PayrollAdjustmentRepository : Repository<PayrollAdjustment>, IPayrollAdjustmentRepository
{
    private PaymentsDbContext PaymentsContext => (PaymentsDbContext)Context;

    public PayrollAdjustmentRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }

    public Task<List<PayrollAdjustment>> GetForPeriodAsync(int year, int month, CancellationToken cancellationToken = default)
    {
        return PaymentsContext.PayrollAdjustments
            .Where(a => a.SalaryYear == year && a.SalaryMonth == month)
            .ToListAsync(cancellationToken);
    }
}
