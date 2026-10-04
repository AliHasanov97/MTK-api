using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.Buildings;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

internal sealed class BuildingRepository : Repository<Building>, IBuildingRepository
{
    public BuildingRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }

    public Task<Building?> GetFirstAsync(CancellationToken cancellationToken = default)
    {
        return DbItem.OrderBy(b => b.CreatedAt).FirstOrDefaultAsync(cancellationToken);
    }
}
