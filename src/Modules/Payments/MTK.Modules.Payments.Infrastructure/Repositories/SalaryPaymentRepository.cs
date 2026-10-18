using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Domain.SalaryPayments;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

internal sealed class SalaryPaymentRepository : SearchableRepository<SalaryPayment>, ISalaryPaymentRepository
{
    private PaymentsDbContext PaymentsContext => (PaymentsDbContext)Context;

    public SalaryPaymentRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }

    public Task<SalaryPayment?> GetByIdWithLinesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return PaymentsContext.SalaryPayments
            .Include(s => s.Lines)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public Task<SalaryPayment?> GetByEmployeeAndPeriodAsync(
        Guid employeeId,
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        return PaymentsContext.SalaryPayments
            .FirstOrDefaultAsync(s =>
                s.EmployeeId == employeeId &&
                s.SalaryYear == year &&
                s.SalaryMonth == month, cancellationToken);
    }

    public async Task<List<SalaryPayment>> GetByEmployeeAndPeriodRangeAsync(
        Guid employeeId,
        int fromYear,
        int fromMonth,
        int toYear,
        int toMonth,
        CancellationToken cancellationToken = default)
    {
        return await PaymentsContext.SalaryPayments
            .Where(s => s.EmployeeId == employeeId &&
                        s.Status == SalaryPaymentStatus.Paid &&
                        ((s.SalaryYear == fromYear && s.SalaryMonth >= fromMonth) ||
                         (s.SalaryYear > fromYear && s.SalaryYear < toYear) ||
                         (s.SalaryYear == toYear && s.SalaryMonth <= toMonth)))
            .OrderBy(s => s.SalaryYear)
            .ThenBy(s => s.SalaryMonth)
            .ToListAsync(cancellationToken);
    }
}
