using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

internal sealed class ChargeRepository : SearchableRepository<Charge>, IChargeRepository
{
    private PaymentsDbContext PaymentsContext => (PaymentsDbContext)Context;

    public ChargeRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Charge>> GetByOwnerIdAsync(
        Guid ownerId,
        CancellationToken cancellationToken = default)
    {
        return await PaymentsContext.Charges
            .Where(c => c.OwnerId == ownerId)
            .OrderBy(c => c.IssuedOn)
            .ThenBy(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Charge>> GetByPropertyIdAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        return await PaymentsContext.Charges
            .Where(c => c.PropertyId == propertyId)
            .OrderBy(c => c.IssuedOn)
            .ThenBy(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Ödəniş/avans hansı borca əvvəl tətbiq olunacağını bu sıra müəyyən edir:
    /// ən köhnə borc (IssuedOn) → yaranma anı → əmlak tipi → Id.
    ///
    /// Son üç meyar yalnız tam eyni yaşlı borclar üçündür (eyni generasiya
    /// yürüşündə yarananlar kimi) — sıra deterministik olsun deyə.
    /// </summary>
    public async Task<IEnumerable<Charge>> GetUnpaidChargesAsync(
        Guid ownerId,
        CancellationToken cancellationToken = default)
    {
        return await OrderedUnpaid(PaymentsContext.Charges.Where(c => c.OwnerId == ownerId))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Charge>> GetUnpaidChargesAsync(
        Guid ownerId,
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        return await OrderedUnpaid(
                PaymentsContext.Charges.Where(c => c.OwnerId == ownerId && c.PropertyId == propertyId))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ChargeExistsForPeriodAsync(
        Guid ownerId,
        Guid propertyId,
        string period,
        CancellationToken cancellationToken = default)
    {
        return await PaymentsContext.Charges
            .AnyAsync(c => c.OwnerId == ownerId && c.PropertyId == propertyId && c.Period == period, cancellationToken);
    }

    public async Task<Dictionary<Guid, decimal>> GetTotalAmountByOwnerIdsAsync(
        IReadOnlyCollection<Guid> ownerIds,
        CancellationToken cancellationToken = default)
    {
        if (ownerIds.Count == 0)
        {
            return new Dictionary<Guid, decimal>();
        }

        var totals = await PaymentsContext.Charges
            .Where(c => ownerIds.Contains(c.OwnerId))
            .GroupBy(c => c.OwnerId)
            .Select(g => new { OwnerId = g.Key, Total = g.Sum(c => c.Amount) })
            .ToListAsync(cancellationToken);

        return totals.ToDictionary(t => t.OwnerId, t => t.Total);
    }

    private static IOrderedQueryable<Charge> OrderedUnpaid(IQueryable<Charge> query)
    {
        return query
            .Where(c => c.Status != ChargeStatus.Paid)
            .OrderBy(c => c.IssuedOn)
            .ThenBy(c => c.CreatedAt)
            .ThenBy(c => c.PropertyType)
            .ThenBy(c => c.Id);
    }
}
