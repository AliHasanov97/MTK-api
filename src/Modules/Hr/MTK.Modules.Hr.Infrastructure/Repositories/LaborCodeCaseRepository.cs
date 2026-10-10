using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.LaborCodeCases;
using Microsoft.EntityFrameworkCore;

using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class LaborCodeCaseRepository : SearchableRepository<LaborCodeCase>, ILaborCodeCaseRepository
{
    public LaborCodeCaseRepository(HrDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<LaborCodeCase>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<LaborCodeCase>()
            .AsNoTracking()
            .Where(l => l.ParentId == parentId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LaborCodeCase>> GetRootCasesAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<LaborCodeCase>()
            .AsNoTracking()
            .Where(l => l.ParentId == null)
            .ToListAsync(cancellationToken);
    }

    public override async Task<List<LaborCodeCase>> SearchAsync(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria,
        string? searchTerm,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<LaborCodeCase> query = ApplyFiltersAndSort(filters, sortCriteria)
            .Include(l => l.Parent);

        var searchTermQuery = GetSearchTerm(searchTerm);
        if (!string.IsNullOrWhiteSpace(searchTermQuery))
        {
            query = query.Where(l => l.SearchVector.Matches(EF.Functions.ToTsQuery(searchTermQuery)));
        }

        query = ApplyPages(query, page, pageSize);
        return await query.ToListAsync(cancellationToken);
    }
}
