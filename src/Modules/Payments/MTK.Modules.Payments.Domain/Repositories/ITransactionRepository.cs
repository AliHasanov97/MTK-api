using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Transactions;

namespace MTK.Modules.Payments.Domain.Repositories;

public interface ITransactionRepository : IRepository<Transaction>
{
    /// <summary>All-time sum per direction (Income/Expense) — the source CompanyBalance recalculates from.</summary>
    Task<Dictionary<TransactionDirection, decimal>> GetTotalsByDirectionAsync(CancellationToken cancellationToken = default);
}
