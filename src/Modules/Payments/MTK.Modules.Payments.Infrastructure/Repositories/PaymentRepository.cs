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
/// sakin-facing axtarış/toplu sorğular <c>PartyType == Owner</c> ilə məhdudlaşdırılır.
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
            .Where(p => p.PartyType == PartyType.Owner);
    }

    public async Task<IEnumerable<Payment>> GetByOwnerIdAsync(
        Guid ownerId,
        CancellationToken cancellationToken = default)
    {
        return await PaymentsContext.Payments
            .Where(p => p.PartyType == PartyType.Owner && p.PartyId == ownerId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetByPropertyIdAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        return await PaymentsContext.Payments
            .Where(p => p.PropertyId == propertyId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetByPartyIdAsync(
        PartyType partyType,
        Guid partyId,
        CancellationToken cancellationToken = default)
    {
        return await PaymentsContext.Payments
            .Where(p => p.PartyType == partyType && p.PartyId == partyId)
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
                p.PartyType == PartyType.Owner &&
                ownerIds.Contains(p.PartyId) &&
                p.Status == PaymentStatus.Completed)
            .GroupBy(p => p.PartyId)
            .Select(g => new { OwnerId = g.Key, Total = g.Sum(p => p.Amount) })
            .ToListAsync(cancellationToken);

        return totals.ToDictionary(t => t.OwnerId, t => t.Total);
    }
}
