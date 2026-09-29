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
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Charge>> GetByPropertyIdAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        return await PaymentsContext.Charges
            .Where(c => c.PropertyId == propertyId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Charge>> GetUnpaidChargesAsync(
        Guid ownerId,
        CancellationToken cancellationToken = default)
    {
        return await PaymentsContext.Charges
            .Where(c => c.OwnerId == ownerId)
            .Where(c => c.Status != ChargeStatus.Paid)
            .OrderBy(c => c.Period)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Charge>> GetUnpaidChargesAsync(
        Guid ownerId,
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        return await PaymentsContext.Charges
            .Where(c => c.OwnerId == ownerId && c.PropertyId == propertyId)
            .Where(c => c.Status != ChargeStatus.Paid)
            .OrderBy(c => c.Period)
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
}
