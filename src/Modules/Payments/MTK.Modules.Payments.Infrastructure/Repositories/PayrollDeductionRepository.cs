using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.Payroll;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

internal sealed class PayrollDeductionRepository : Repository<PayrollDeduction>, IPayrollDeductionRepository
{
    private PaymentsDbContext PaymentsContext => (PaymentsDbContext)Context;

    public PayrollDeductionRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }

    public Task<List<PayrollDeduction>> GetActiveAsync(DateTimeOffset asOfDate, CancellationToken cancellationToken = default)
    {
        return PaymentsContext.PayrollDeductions
            .Where(d => d.StartDate <= asOfDate)
            .ToListAsync(cancellationToken);
    }
}
