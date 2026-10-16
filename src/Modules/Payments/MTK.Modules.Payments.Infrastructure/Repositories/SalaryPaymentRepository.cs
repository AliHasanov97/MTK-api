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
}
