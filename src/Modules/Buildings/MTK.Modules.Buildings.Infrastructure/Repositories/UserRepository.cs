using MTK.Common.Infrastructure.Database;
using MTK.Modules.Buildings.Domain.Repositories;
using MTK.Modules.Buildings.Domain.Users;
using MTK.Modules.Buildings.Infrastructure.Database;

namespace MTK.Modules.Buildings.Infrastructure.Repositories;

internal sealed class UserRepository : SearchableRepository<User>, IUserRepository
{
    public UserRepository(BuildingsDbContext dbContext) : base(dbContext)
    {
    }
}
