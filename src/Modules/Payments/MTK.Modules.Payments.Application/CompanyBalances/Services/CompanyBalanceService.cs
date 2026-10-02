using MTK.Modules.Payments.Domain.CompanyBalances;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Domain.Transactions;

namespace MTK.Modules.Payments.Application.CompanyBalances.Services;

internal sealed class CompanyBalanceService(
    ITransactionRepository transactionRepository,
    ICompanyBalanceRepository companyBalanceRepository) : ICompanyBalanceService
{
    public async Task RecalculateAsync(CancellationToken cancellationToken = default)
    {
        var totalsByDirection = await transactionRepository.GetTotalsByDirectionAsync(cancellationToken);
        var totalIncome = totalsByDirection.GetValueOrDefault(TransactionDirection.Income);
        var totalExpense = totalsByDirection.GetValueOrDefault(TransactionDirection.Expense);

        var balance = await companyBalanceRepository.GetAsync(cancellationToken);
        if (balance is null)
        {
            balance = CompanyBalance.Create();
            companyBalanceRepository.Add(balance);
        }
        else if (balance.TotalIncome == totalIncome && balance.TotalExpense == totalExpense)
        {
            // Dəyişiklik yoxdur — boş yerə UPDATE (və audit qeydi) yazmayaq.
            return;
        }

        balance.SetTotals(totalIncome, totalExpense);
    }
}
