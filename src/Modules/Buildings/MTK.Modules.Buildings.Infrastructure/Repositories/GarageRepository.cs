using Microsoft.EntityFrameworkCore;
using MTK.Modules.Buildings.Domain.Garages;
using MTK.Modules.Buildings.Domain.Repositories;
using MTK.Modules.Buildings.Infrastructure.Database;

namespace MTK.Modules.Buildings.Infrastructure.Repositories;

internal sealed class GarageRepository : Repository<Garage>, IGarageRepository
{
    public GarageRepository(BuildingsDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Garage>> GetByOwnerIdAsync(
        Guid ownerId,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.Garages
            .Include(g => g.Owner)
            .Where(g => g.OwnerId == ownerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNumberAsync(
        string garageNumber,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.Garages
            .AnyAsync(g => g.GarageNumber == garageNumber, cancellationToken);
    }

    public override async Task<Garage?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.Garages
            .Include(g => g.Owner)
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }
}
