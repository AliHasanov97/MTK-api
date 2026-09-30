using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

internal sealed class PaymentRepository : SearchableRepository<Payment>, IPaymentRepository
{
    private PaymentsDbContext PaymentsContext => (PaymentsDbContext)Context;

    public PaymentRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
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
            .Where(p => p.PropertyId == propertyId)
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
            .Where(p => ownerIds.Contains(p.OwnerId) && p.Status == PaymentStatus.Completed)
            .GroupBy(p => p.OwnerId)
            .Select(g => new { OwnerId = g.Key, Total = g.Sum(p => p.Amount) })
            .ToListAsync(cancellationToken);

        return totals.ToDictionary(t => t.OwnerId, t => t.Total);
    }
}
