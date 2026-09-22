using Microsoft.EntityFrameworkCore;
using MTK.Modules.Buildings.Domain.OwnershipHistories;
using MTK.Modules.Buildings.Domain.Repositories;
using MTK.Modules.Buildings.Infrastructure.Database;

namespace MTK.Modules.Buildings.Infrastructure.Repositories;

internal sealed class OwnershipHistoryRepository
    : Repository<OwnershipHistory>, IOwnershipHistoryRepository
{
    public OwnershipHistoryRepository(BuildingsDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<OwnershipHistory>> GetByApartmentIdAsync(
        Guid apartmentId,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.OwnershipHistories
            .Include(oh => oh.Apartment)
            .Include(oh => oh.PreviousOwner)
            .Include(oh => oh.NewOwner)
            .Where(oh => oh.ApartmentId == apartmentId)
            .OrderByDescending(oh => oh.TransferDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<OwnershipHistory?> GetCurrentOwnershipAsync(
        Guid apartmentId,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.OwnershipHistories
            .Include(oh => oh.Apartment)
            .Include(oh => oh.PreviousOwner)
            .Include(oh => oh.NewOwner)
            .Where(oh => oh.ApartmentId == apartmentId)
            .OrderByDescending(oh => oh.TransferDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public override async Task<OwnershipHistory?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.OwnershipHistories
            .Include(oh => oh.Apartment)
            .Include(oh => oh.PreviousOwner)
            .Include(oh => oh.NewOwner)
            .FirstOrDefaultAsync(oh => oh.Id == id, cancellationToken);
    }
}
