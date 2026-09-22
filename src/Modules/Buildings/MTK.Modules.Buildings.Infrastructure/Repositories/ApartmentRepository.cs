using Microsoft.EntityFrameworkCore;
using MTK.Modules.Buildings.Domain.Apartments;
using MTK.Modules.Buildings.Domain.Repositories;
using MTK.Modules.Buildings.Infrastructure.Database;

namespace MTK.Modules.Buildings.Infrastructure.Repositories;

internal sealed class ApartmentRepository : Repository<Apartment>, IApartmentRepository
{
    public ApartmentRepository(BuildingsDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Apartment>> GetByBuildingIdAsync(
        Guid buildingId,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.Apartments
            .Include(a => a.Building)
            .Include(a => a.CurrentOwner)
            .Include(a => a.Garages)
            .Where(a => a.BuildingId == buildingId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Apartment>> GetByOwnerIdAsync(
        Guid ownerId,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.Apartments
            .Include(a => a.Building)
            .Include(a => a.CurrentOwner)
            .Include(a => a.Garages)
            .Where(a => a.CurrentOwnerId == ownerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Apartment>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await DbContext.Apartments
            .Include(a => a.Building)
            .Include(a => a.CurrentOwner)
            .Include(a => a.Garages)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNumberAsync(
        Guid buildingId,
        string apartmentNumber,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.Apartments
            .AnyAsync(
                a => a.BuildingId == buildingId && a.ApartmentNumber == apartmentNumber,
                cancellationToken);
    }

    public override async Task<Apartment?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.Apartments
            .Include(a => a.Building)
            .Include(a => a.CurrentOwner)
            .Include(a => a.Garages)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }
}
