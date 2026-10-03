using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Domain.Users;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

internal sealed class UserRepository : SearchableRepository<User>, IUserRepository
{
    public UserRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }
}
