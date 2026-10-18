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

    public async Task<List<PayslipWithPeriodDto>> GetPayslipsByEmployeeAndPeriodRangeAsync(
        Guid employeeId,
        int fromYear,
        int fromMonth,
        int toYear,
        int toMonth,
        CancellationToken cancellationToken = default)
    {
        return await PaymentsContext.PayrollRuns
            .Where(r => ((r.Year == fromYear && r.Month >= fromMonth) ||
                         (r.Year > fromYear && r.Year < toYear) ||
                         (r.Year == toYear && r.Month <= toMonth)))
            .SelectMany(r => r.Payslips
                .Where(p => p.EmployeeId == employeeId)
                .Select(p => new PayslipWithPeriodDto(
                    r.Year,
                    r.Month,
                    p.BaseSalary,
                    p.BonusAmount,
                    p.AdjustmentAmount,
                    p.GrossSalary)))
            .OrderBy(p => p.Year)
            .ThenBy(p => p.Month)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<PayrollRun>> GetAllPayrollRunsWithPayslipsByEmployeeAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return await PaymentsContext.PayrollRuns
            .Include(r => r.Payslips.Where(p => p.EmployeeId == employeeId))
            .Where(r => r.Payslips.Any(p => p.EmployeeId == employeeId))
            .OrderByDescending(r => r.Year)
            .ThenByDescending(r => r.Month)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<PayrollRun>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await PaymentsContext.PayrollRuns
            .OrderByDescending(r => r.Year)
            .ThenByDescending(r => r.Month)
            .ToListAsync(cancellationToken);
    }
}
