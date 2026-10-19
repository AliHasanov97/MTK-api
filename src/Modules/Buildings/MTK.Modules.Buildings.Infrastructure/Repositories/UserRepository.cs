using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Buildings.Domain.Repositories;
using MTK.Modules.Buildings.Domain.Users;
using MTK.Modules.Buildings.Infrastructure.Database;

namespace MTK.Modules.Buildings.Infrastructure.Repositories;

internal sealed class UserRepository : SearchableRepository<User>, IUserRepository
{
    private BuildingsDbContext BuildingsContext => (BuildingsDbContext)Context;

    public UserRepository(BuildingsDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Dictionary<Guid, string>> GetUserNamesByIdsAsync(
        IEnumerable<Guid> userIds,
        CancellationToken cancellationToken = default)
    {
        var ids = userIds.Distinct().ToList();
        if (!ids.Any())
        {
            return new Dictionary<Guid, string>();
        }

        return await BuildingsContext.Users
            .Where(u => ids.Contains(u.Id))
            .Select(u => new { u.Id, u.FirstName, u.LastName })
            .ToDictionaryAsync(
                u => u.Id,
                u => $"{u.FirstName} {u.LastName}",
                cancellationToken);
    }
}
