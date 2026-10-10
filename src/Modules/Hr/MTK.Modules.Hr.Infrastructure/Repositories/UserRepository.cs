using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.Users;
using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class UserRepository : SearchableRepository<User>, IUserRepository
{
    public UserRepository(HrDbContext dbContext) : base(dbContext)
    {
    }
}
