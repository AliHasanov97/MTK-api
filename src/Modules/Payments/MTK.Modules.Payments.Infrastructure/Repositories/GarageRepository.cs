using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.Garages;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

internal sealed class GarageRepository : Repository<Garage>, IGarageRepository
{
    public GarageRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }
}
