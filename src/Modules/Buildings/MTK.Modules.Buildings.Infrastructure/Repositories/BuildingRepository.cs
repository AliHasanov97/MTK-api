using Microsoft.EntityFrameworkCore;
using MTK.Modules.Buildings.Domain.Buildings;
using MTK.Modules.Buildings.Domain.Repositories;
using MTK.Modules.Buildings.Infrastructure.Database;

namespace MTK.Modules.Buildings.Infrastructure.Repositories;

internal sealed class BuildingRepository : Repository<Building>, IBuildingRepository
{
    public BuildingRepository(BuildingsDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Building>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await DbContext.Buildings
            .Include(b => b.Apartments)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.Buildings
            .AnyAsync(b => b.Name == name, cancellationToken);
    }

    public override async Task<Building?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.Buildings
            .Include(b => b.Apartments)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }
}
