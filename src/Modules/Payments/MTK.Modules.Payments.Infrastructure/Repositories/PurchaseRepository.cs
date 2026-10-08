using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.Purchases;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

internal sealed class PurchaseRepository : SearchableRepository<Purchase>, IPurchaseRepository
{
    private PaymentsDbContext PaymentsContext => (PaymentsDbContext)Context;

    public PurchaseRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }

    public Task<Purchase?> GetWithLinesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return PaymentsContext.Purchases
            .Include(p => p.Lines)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public Task<List<Purchase>> GetReceivedByNomenclatureIdAsync(
        Guid nomenclatureId,
        CancellationToken cancellationToken = default)
    {
        return PaymentsContext.Purchases
            .Where(p => p.Status == PurchaseStatus.Received && p.Lines.Any(l => l.NomenclatureId == nomenclatureId))
            .Include(p => p.Lines)
            .OrderByDescending(p => p.PurchaseDate)
            .ToListAsync(cancellationToken);
    }
}
