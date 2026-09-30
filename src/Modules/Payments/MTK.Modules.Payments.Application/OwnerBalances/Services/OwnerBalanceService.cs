using MTK.Modules.Payments.Domain.OwnerBalances;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.OwnerBalances.Services;

internal sealed class OwnerBalanceService(
    IChargeRepository chargeRepository,
    IPaymentRepository paymentRepository,
    IOwnerBalanceRepository ownerBalanceRepository) : IOwnerBalanceService
{
    public async Task RecalculateAsync(
        IReadOnlyCollection<Guid> ownerIds,
        CancellationToken cancellationToken = default)
    {
        var ids = ownerIds.Where(id => id != Guid.Empty).Distinct().ToList();
        if (ids.Count == 0)
        {
            return;
        }

        // İki toplu sorğu — sahib başına ayrı-ayrı getməsin deyə.
        var totalDebtByOwner = await chargeRepository.GetTotalAmountByOwnerIdsAsync(ids, cancellationToken);
        var totalPaidByOwner = await paymentRepository.GetCompletedTotalByOwnerIdsAsync(ids, cancellationToken);
        var balances = await ownerBalanceRepository.GetByOwnerIdsAsync(ids, cancellationToken);

        foreach (var ownerId in ids)
        {
            decimal totalDebt = totalDebtByOwner.GetValueOrDefault(ownerId);
            decimal totalPaid = totalPaidByOwner.GetValueOrDefault(ownerId);

            if (!balances.TryGetValue(ownerId, out var balance))
            {
                balance = OwnerBalance.Create(ownerId);
                ownerBalanceRepository.Add(balance);
            }
            else if (balance.TotalDebt == totalDebt && balance.TotalPaid == totalPaid)
            {
                // Dəyişiklik yoxdur — boş yerə UPDATE (və audit qeydi) yazmayaq.
                continue;
            }

            balance.SetTotals(totalDebt, totalPaid);
        }
    }
}
