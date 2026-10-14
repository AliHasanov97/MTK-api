using Microsoft.EntityFrameworkCore;
using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.Parties;
using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

/// <summary>
/// Ödənişlər üzrə repository. <see cref="Payment"/> artıq hər iki tərəfi saxlayır;
/// sakin-facing axtarış/toplu sorğular <c>OwnerId != null</c> ilə məhdudlaşdırılır.
/// </summary>
internal sealed class PaymentRepository : SearchableRepository<Payment>, IPaymentRepository
{
    private PaymentsDbContext PaymentsContext => (PaymentsDbContext)Context;

    public PaymentRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }

    protected override IQueryable<Payment> ApplyFiltersAndSort(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria)
    {
        return base.ApplyFiltersAndSort(filters, sortCriteria)
            .Where(p => p.OwnerId != null);
    }

    public async Task<IEnumerable<Payment>> GetByOwnerIdAsync(
        Guid ownerId,
        CancellationToken cancellationToken = default)
    {
        return await PaymentsContext.Payments
            .Where(p => p.OwnerId == ownerId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetByPropertyIdAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        return await PaymentsContext.Payments
            .Where(p => p.ApartmentId == propertyId || p.GarageId == propertyId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Payment>> GetCompletedByPartyIdsAsync(
        PartyType partyType,
        IReadOnlyCollection<Guid> partyIds,
        CancellationToken cancellationToken = default)
    {
        if (partyIds.Count == 0)
        {
            return [];
        }

        var ids = partyIds.ToList();
        var query = partyType switch
        {
            PartyType.Owner => PaymentsContext.Payments.Where(p => p.OwnerId != null && ids.Contains(p.OwnerId.Value)),
            PartyType.Customer => PaymentsContext.Payments.Where(p => p.CustomerId != null && ids.Contains(p.CustomerId.Value)),
            _ => PaymentsContext.Payments.Where(p => p.VendorId != null && ids.Contains(p.VendorId.Value)),
        };

        // Ödənişin özü dəyişdirilmir (yalnız paylanma yazılır) — izləməyə ehtiyac yoxdur
        return await query
            .AsNoTracking()
            .Where(p => p.Status == PaymentStatus.Completed)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetByPartyIdAsync(
        PartyType partyType,
        Guid partyId,
        CancellationToken cancellationToken = default)
    {
        var query = partyType switch
        {
            PartyType.Owner => PaymentsContext.Payments.Where(p => p.OwnerId == partyId),
            PartyType.Customer => PaymentsContext.Payments.Where(p => p.CustomerId == partyId),
            _ => PaymentsContext.Payments.Where(p => p.VendorId == partyId),
        };

        return await query
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<Guid, decimal>> GetCompletedTotalByOwnerIdsAsync(
        IReadOnlyCollection<Guid> ownerIds,
        CancellationToken cancellationToken = default)
    {
        if (ownerIds.Count == 0)
        {
            return new Dictionary<Guid, decimal>();
        }

        var totals = await PaymentsContext.Payments
            .Where(p =>
                p.OwnerId != null &&
                ownerIds.Contains(p.OwnerId!.Value) &&
                p.Status == PaymentStatus.Completed)
            .GroupBy(p => p.OwnerId!.Value)
            .Select(g => new { OwnerId = g.Key, Total = g.Sum(p => p.Amount) })
            .ToListAsync(cancellationToken);

        return totals.ToDictionary(t => t.OwnerId, t => t.Total);
    }
}
