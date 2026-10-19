using Microsoft.EntityFrameworkCore;
using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Communications.Domain.Users;
using MTK.Modules.Communications.Infrastructure.Database;

namespace MTK.Modules.Communications.Infrastructure.Repositories;

internal sealed class UserRepository : SearchableRepository<User>, IUserRepository
{
    public UserRepository(CommunicationsDbContext context) : base(context)
    {
    }

    public override async Task<User?> GetByIdDefaultAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<User>()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public override async Task<List<User>> SearchAsync(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria,
        string? searchTerm,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<User> query = ApplyFiltersAndSort(filters, sortCriteria);

        var searchTermQuery = GetSearchTerm(searchTerm);
        if (!string.IsNullOrWhiteSpace(searchTermQuery))
        {
            query = query.Where(u => u.SearchVector.Matches(EF.Functions.ToTsQuery(searchTermQuery)));
        }

        query = ApplyPages(query, page, pageSize);
        return await query.ToListAsync(cancellationToken);
    }
}
