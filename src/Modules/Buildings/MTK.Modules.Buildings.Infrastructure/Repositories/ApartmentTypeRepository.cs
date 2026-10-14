using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Buildings.Domain.ApartmentTypes;
using MTK.Modules.Buildings.Domain.Repositories;
using MTK.Modules.Buildings.Infrastructure.Database;

namespace MTK.Modules.Buildings.Infrastructure.Repositories;

internal sealed class ApartmentTypeRepository : Repository<ApartmentType>, IApartmentTypeRepository
{
    private BuildingsDbContext BuildingsContext => (BuildingsDbContext)Context;

    public ApartmentTypeRepository(BuildingsDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<ApartmentType>> GetByBuildingIdAsync(
        Guid buildingId,
        CancellationToken cancellationToken = default)
    {
        return await BuildingsContext.ApartmentTypes
            .AsNoTracking()
            .Where(t => t.BuildingId == buildingId)
            .OrderBy(t => t.RoomCount)
            .ThenBy(t => t.AreaSquareMeters)
            .ToListAsync(cancellationToken);
    }
}
