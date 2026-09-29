using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Domain.Transactions;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

internal sealed class TransactionRepository : SearchableRepository<Transaction>, ITransactionRepository
{
    public TransactionRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }
}
