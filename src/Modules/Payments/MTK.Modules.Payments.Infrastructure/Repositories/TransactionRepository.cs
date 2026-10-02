using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Domain.Transactions;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

internal sealed class TransactionRepository : SearchableRepository<Transaction>, ITransactionRepository
{
    private PaymentsDbContext PaymentsContext => (PaymentsDbContext)Context;

    public TransactionRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Dictionary<TransactionDirection, decimal>> GetTotalsByDirectionAsync(
        CancellationToken cancellationToken = default)
    {
        var totals = await PaymentsContext.Transactions
            .GroupBy(t => t.Direction)
            .Select(g => new { Direction = g.Key, Total = g.Sum(t => t.Amount) })
            .ToListAsync(cancellationToken);

        return totals.ToDictionary(t => t.Direction, t => t.Total);
    }
}
