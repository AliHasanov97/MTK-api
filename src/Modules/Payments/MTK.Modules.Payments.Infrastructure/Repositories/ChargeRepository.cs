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
/// saxlayır; buna görə bütün sorğular <c>OwnerId != null</c> ilə məhdudlaşdırılır.
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
            .Where(c => c.OwnerId != null);
    }

    // Axtarış sakinin adı, mənzil/qaraj nömrəsi, dövr və təsvir üzrə işləyir (yalnız
    // Period/Description-dan ibarət tsvector kifayət etmir). Sözlər arasında AND:
    // hər söz bu sahələrdən ən azı birində olmalıdır.
    public override async Task<List<Charge>> SearchAsync(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria,
        string? searchTerm,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = ApplySearchTerm(ApplyFiltersAndSort(filters, sortCriteria), searchTerm);
        query = ApplyPages(query, page, pageSize);
        return await query.ToListAsync(cancellationToken);
    }

    public override async Task<int> CountAsync(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria,
        string? searchTerm,
        CancellationToken cancellationToken = default)
    {
        return await ApplySearchTerm(ApplyFiltersAndSort(filters, sortCriteria), searchTerm)
            .CountAsync(cancellationToken);
    }

    private static IQueryable<Charge> ApplySearchTerm(IQueryable<Charge> query, string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return query;

        foreach (var word in searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var escaped = word.Replace(@"\", @"\\").Replace("%", @"\%").Replace("_", @"\_");
            var pattern = $"%{escaped}%";
            query = query.Where(c =>
                (c.Owner != null && EF.Functions.ILike(c.Owner.FullName, pattern))
                || (c.Apartment != null && EF.Functions.ILike(c.Apartment.ApartmentNumber, pattern))
                || (c.Garage != null && EF.Functions.ILike(c.Garage.GarageNumber, pattern))
                || (c.Period != null && EF.Functions.ILike(c.Period, pattern))
                || (c.Description != null && EF.Functions.ILike(c.Description, pattern)));
        }

        return query;
    }

    public async Task<IEnumerable<Charge>> GetByOwnerIdAsync(
        Guid ownerId,
        CancellationToken cancellationToken = default)
    {
        return await PaymentsContext.Charges
            .Include(c => c.Apartment).ThenInclude(a => a!.Building)
            .Include(c => c.Garage)
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
            .Include(c => c.Apartment).ThenInclude(a => a!.Building)
            .Include(c => c.Garage)
            .Where(c => c.ApartmentId == propertyId || c.GarageId == propertyId)
            .OrderBy(c => c.IssuedOn)
            .ThenBy(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Ödəniş/avans hansı borca əvvəl tətbiq olunacağını bu sıra müəyyən edir:
    /// ən köhnə borc (IssuedOn) → yaranma anı → əmlak (mənzil/qaraj) → Id.
    /// </summary>
    public async Task<IEnumerable<Charge>> GetUnpaidChargesAsync(
        Guid ownerId,
        CancellationToken cancellationToken = default)
    {
        return await OrderedUnpaid(
                PaymentsContext.Charges.Where(c => c.OwnerId == ownerId))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Charge>> GetUnpaidChargesAsync(
        Guid ownerId,
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        return await OrderedUnpaid(
                PaymentsContext.Charges.Where(c =>
                    c.OwnerId == ownerId && (c.ApartmentId == propertyId || c.GarageId == propertyId)))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Charge>> GetUnpaidChargesByPartyAsync(
        PartyType partyType,
        Guid partyId,
        CancellationToken cancellationToken = default)
    {
        var query = partyType == PartyType.Owner
            ? PaymentsContext.Charges.Where(c => c.OwnerId == partyId)
            : PaymentsContext.Charges.Where(c => c.VendorId == partyId);

        return await OrderedUnpaid(query).ToListAsync(cancellationToken);
    }

    public async Task<bool> ChargeExistsForPeriodAsync(
        Guid propertyId,
        string period,
        CancellationToken cancellationToken = default)
    {
        return await PaymentsContext.Charges
            .AnyAsync(c =>
                c.OwnerId != null &&
                (c.ApartmentId == propertyId || c.GarageId == propertyId) &&
                c.Period == period &&
                c.Status != ChargeStatus.Cancelled,
                cancellationToken);
    }

    public async Task<HashSet<Guid>> GetPropertyIdsWithChargeForPeriodAsync(
        string period,
        bool includeCancelled,
        CancellationToken cancellationToken = default)
    {
        var query = PaymentsContext.Charges
            .Where(c => c.OwnerId != null && c.Period == period && (includeCancelled || c.Status != ChargeStatus.Cancelled));

        var apartmentIds = await query.Where(c => c.ApartmentId != null).Select(c => c.ApartmentId!.Value).ToListAsync(cancellationToken);
        var garageIds = await query.Where(c => c.GarageId != null).Select(c => c.GarageId!.Value).ToListAsync(cancellationToken);

        return [.. apartmentIds, .. garageIds];
    }

    public async Task<List<OwnerChargeYearItem>> GetOwnerChargeItemsByYearAsync(
        int year,
        IReadOnlyCollection<Guid>? propertyIds = null,
        CancellationToken cancellationToken = default)
    {
        var prefix = $"{year}-";
        var query = PaymentsContext.Charges
            .AsNoTracking()
            .Where(c => c.OwnerId != null && c.Period != null && c.Period.StartsWith(prefix));

        if (propertyIds is not null)
        {
            var ids = propertyIds.ToList();
            query = query.Where(c => (c.ApartmentId != null && ids.Contains(c.ApartmentId.Value)) ||
                                     (c.GarageId != null && ids.Contains(c.GarageId.Value)));
        }

        return await query
            .Select(c => new OwnerChargeYearItem(
                (c.ApartmentId ?? c.GarageId)!.Value,
                c.Period!,
                c.Amount,
                c.PaidAmount,
                c.Status))
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
                c.OwnerId != null &&
                (c.ApartmentId != null || c.GarageId != null) &&
                (propertyIds.Contains(c.ApartmentId!.Value) || propertyIds.Contains(c.GarageId!.Value)) &&
                c.Status != ChargeStatus.Cancelled)
            .GroupBy(c => (c.ApartmentId ?? c.GarageId)!.Value)
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
            .Where(c => c.OwnerId != null && ownerIds.Contains(c.OwnerId!.Value))
            .GroupBy(c => c.OwnerId!.Value)
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
            .ThenBy(c => c.ApartmentId ?? c.GarageId)
            .ThenBy(c => c.Id);
    }
}
