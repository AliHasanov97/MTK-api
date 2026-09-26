using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Buildings.Domain.Owners;
using MTK.Modules.Buildings.Domain.Repositories;
using MTK.Modules.Buildings.Infrastructure.Database;

namespace MTK.Modules.Buildings.Infrastructure.Repositories;

internal sealed class OwnerRepository : SearchableRepository<Owner>, IOwnerRepository
{
    private BuildingsDbContext BuildingsContext => (BuildingsDbContext)Context;

    public OwnerRepository(BuildingsDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Owner?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await BuildingsContext.Owners
            .Include(o => o.OwnedApartments)
            .Include(o => o.OwnedGarages)
            .FirstOrDefaultAsync(o => o.UserId == userId, cancellationToken);
    }

    public async Task<Owner?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return await BuildingsContext.Owners
            .Include(o => o.OwnedApartments)
            .Include(o => o.OwnedGarages)
            .FirstOrDefaultAsync(o => o.Email == email, cancellationToken);
    }

    public async Task<IEnumerable<Owner>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await BuildingsContext.Owners
            .Include(o => o.OwnedApartments)
            .Include(o => o.OwnedGarages)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await BuildingsContext.Owners
            .AnyAsync(o => o.UserId == userId, cancellationToken);
    }

    public override async Task<Owner?> GetByIdDefaultAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await BuildingsContext.Owners
            .Include(o => o.OwnedApartments)
            .Include(o => o.OwnedGarages)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }
}
