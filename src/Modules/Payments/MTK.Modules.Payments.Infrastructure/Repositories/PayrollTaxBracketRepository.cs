using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.Payroll;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

internal sealed class PayrollTaxBracketRepository : Repository<PayrollTaxBracket>, IPayrollTaxBracketRepository
{
    private PaymentsDbContext PaymentsContext => (PaymentsDbContext)Context;

    public PayrollTaxBracketRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }

    public Task<List<PayrollTaxBracket>> GetEffectiveBracketsAsync(DateTimeOffset asOfDate, CancellationToken cancellationToken = default)
    {
        return PaymentsContext.PayrollTaxBrackets
            .Where(b => b.EffectiveFrom <= asOfDate && (b.EffectiveTo == null || b.EffectiveTo > asOfDate))
            .ToListAsync(cancellationToken);
    }
}
