using MTK.Common.Infrastructure.Database;
using MTK.Modules.Warehouse.Domain.Users;
using MTK.Modules.Warehouse.Infrastructure.Database;

namespace MTK.Modules.Warehouse.Infrastructure.Repositories;

internal sealed class UserRepository : SearchableRepository<User>, IUserRepository
{
    public UserRepository(WarehouseDbContext dbContext) : base(dbContext)
    {
    }
}
