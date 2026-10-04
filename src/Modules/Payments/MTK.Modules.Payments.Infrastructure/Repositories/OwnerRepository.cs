using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.Owners;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

internal sealed class OwnerRepository : Repository<Owner>, IOwnerRepository
{
    public OwnerRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }
}
