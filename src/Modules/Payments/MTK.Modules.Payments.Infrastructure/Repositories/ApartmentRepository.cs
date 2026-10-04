using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.Apartments;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

internal sealed class ApartmentRepository : Repository<Apartment>, IApartmentRepository
{
    private PaymentsDbContext PaymentsContext => (PaymentsDbContext)Context;

    public ApartmentRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }

    public override async Task<Apartment?> GetByIdDefaultAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await PaymentsContext.Apartments
            .Include(a => a.Building)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public Task<List<Apartment>> ListFromIdsWithBuildingAsync(List<Guid> ids, CancellationToken cancellationToken = default)
    {
        return PaymentsContext.Apartments
            .Include(a => a.Building)
            .Where(a => ids.Contains(a.Id))
            .ToListAsync(cancellationToken);
    }
}
