using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.Payroll;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

internal sealed class PayrollRunRepository : Repository<PayrollRun>, IPayrollRunRepository
{
    private PaymentsDbContext PaymentsContext => (PaymentsDbContext)Context;

    public PayrollRunRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }

    public Task<PayrollRun?> GetWithPayslipsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return PaymentsContext.PayrollRuns
            .Include(r => r.Payslips)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public Task<PayrollRun?> GetByPeriodAsync(int year, int month, CancellationToken cancellationToken = default)
    {
        return PaymentsContext.PayrollRuns
            .FirstOrDefaultAsync(r => r.Year == year && r.Month == month, cancellationToken);
    }

    public Task<PayrollRun?> GetByPeriodWithPayslipsAsync(int year, int month, CancellationToken cancellationToken = default)
    {
        return PaymentsContext.PayrollRuns
            .Include(r => r.Payslips)
            .FirstOrDefaultAsync(r => r.Year == year && r.Month == month, cancellationToken);
    }
}
