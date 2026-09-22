using Microsoft.EntityFrameworkCore;
using MTK.Modules.Buildings.Domain.Owners;
using MTK.Modules.Buildings.Domain.Repositories;
using MTK.Modules.Buildings.Infrastructure.Database;

namespace MTK.Modules.Buildings.Infrastructure.Repositories;

internal sealed class OwnerRepository : Repository<Owner>, IOwnerRepository
{
    public OwnerRepository(BuildingsDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Owner?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.Owners
            .Include(o => o.OwnedApartments)
            .Include(o => o.OwnedGarages)
            .FirstOrDefaultAsync(o => o.UserId == userId, cancellationToken);
    }

    public async Task<IEnumerable<Owner>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await DbContext.Owners
            .Include(o => o.OwnedApartments)
            .Include(o => o.OwnedGarages)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.Owners
            .AnyAsync(o => o.UserId == userId, cancellationToken);
    }

    public override async Task<Owner?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.Owners
            .Include(o => o.OwnedApartments)
            .Include(o => o.OwnedGarages)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }
}
