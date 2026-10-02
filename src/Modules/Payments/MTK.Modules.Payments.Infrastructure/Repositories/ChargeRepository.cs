using Microsoft.EntityFrameworkCore;
using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Parties;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

/// <summary>
/// Sakin borcları üzrə repository. <see cref="Charge"/> artıq hər iki tərəfi
/// saxlayır; buna görə bütün sorğular <c>PartyType == Owner</c> ilə məhdudlaşdırılır.
/// </summary>
internal sealed class ChargeRepository : SearchableRepository<Charge>, IChargeRepository
{
    private PaymentsDbContext PaymentsContext => (PaymentsDbContext)Context;

    public ChargeRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }

    // Axtarış/toplu sorğular avtomatik olaraq yalnız sakin borclarını qaytarsın.
    protected override IQueryable<Charge> ApplyFiltersAndSort(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria)
    {
        return base.ApplyFiltersAndSort(filters, sortCriteria)
            .Where(c => c.PartyType == PartyType.Owner);
    }

    public async Task<IEnumerable<Charge>> GetByOwnerIdAsync(
        Guid ownerId,
        CancellationToken cancellationToken = default)
    {
        return await PaymentsContext.Charges
            .Where(c => c.PartyType == PartyType.Owner && c.PartyId == ownerId)
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
    /// </summary>
    public async Task<IEnumerable<Charge>> GetUnpaidChargesAsync(
        Guid ownerId,
        CancellationToken cancellationToken = default)
    {
        return await OrderedUnpaid(
                PaymentsContext.Charges.Where(c => c.PartyType == PartyType.Owner && c.PartyId == ownerId))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Charge>> GetUnpaidChargesAsync(
        Guid ownerId,
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        return await OrderedUnpaid(
                PaymentsContext.Charges.Where(c =>
                    c.PartyType == PartyType.Owner && c.PartyId == ownerId && c.PropertyId == propertyId))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Charge>> GetUnpaidChargesByPartyAsync(
        PartyType partyType,
        Guid partyId,
        CancellationToken cancellationToken = default)
    {
        return await OrderedUnpaid(
                PaymentsContext.Charges.Where(c => c.PartyType == partyType && c.PartyId == partyId))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ChargeExistsForPeriodAsync(
        Guid propertyId,
        string period,
        CancellationToken cancellationToken = default)
    {
        return await PaymentsContext.Charges
            .AnyAsync(c =>
                c.PartyType == PartyType.Owner &&
                c.PropertyId == propertyId &&
                c.Period == period,
                cancellationToken);
    }

    public async Task<IEnumerable<Charge>> GetOwnerChargesByYearAsync(
        int year,
        CancellationToken cancellationToken = default)
    {
        var prefix = $"{year}-";
        return await PaymentsContext.Charges
            .Where(c => c.PartyType == PartyType.Owner && c.Period != null && c.Period.StartsWith(prefix))
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<Guid, decimal>> GetOutstandingAmountByPropertyIdsAsync(
        IReadOnlyCollection<Guid> propertyIds,
        CancellationToken cancellationToken = default)
    {
        if (propertyIds.Count == 0)
        {
            return new Dictionary<Guid, decimal>();
        }

        var totals = await PaymentsContext.Charges
            .Where(c =>
                c.PartyType == PartyType.Owner &&
                c.PropertyId != null &&
                propertyIds.Contains(c.PropertyId!.Value) &&
                c.Status != ChargeStatus.Cancelled)
            .GroupBy(c => c.PropertyId!.Value)
            .Select(g => new { PropertyId = g.Key, Outstanding = g.Sum(c => c.Amount - c.PaidAmount) })
            .ToListAsync(cancellationToken);

        return totals.ToDictionary(t => t.PropertyId, t => t.Outstanding);
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
            .Where(c => c.PartyType == PartyType.Owner && ownerIds.Contains(c.PartyId))
            .GroupBy(c => c.PartyId)
            .Select(g => new { OwnerId = g.Key, Total = g.Sum(c => c.Amount) })
            .ToListAsync(cancellationToken);

        return totals.ToDictionary(t => t.OwnerId, t => t.Total);
    }

    private static IOrderedQueryable<Charge> OrderedUnpaid(IQueryable<Charge> query)
    {
        return query
            .Where(c => c.Status != ChargeStatus.Paid && c.Status != ChargeStatus.Cancelled)
            .OrderBy(c => c.IssuedOn)
            .ThenBy(c => c.CreatedAt)
            .ThenBy(c => c.PropertyType)
            .ThenBy(c => c.Id);
    }
}
