using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.ApplicationsForChangeOfPosition;
using Microsoft.EntityFrameworkCore;

using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class ApplicationForChangeOfPositionRepository : SearchableRepository<ApplicationForChangeOfPosition>, IApplicationForChangeOfPositionRepository
{
    public ApplicationForChangeOfPositionRepository(HrDbContext context) : base(context)
    {
    }

    public override async Task<ApplicationForChangeOfPosition?> GetByIdDefaultAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<ApplicationForChangeOfPosition>()
            .Include(a => a.Employee)
            .Include(a => a.CurrentJob)
            .Include(a => a.NewJob)
            .Include(a => a.CreatedBy)
            .Include(a => a.OrderForChangeOfPosition)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public override async Task<List<ApplicationForChangeOfPosition>> SearchAsync(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria,
        string? searchTerm,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<ApplicationForChangeOfPosition> query = ApplyFiltersAndSort(filters, sortCriteria)
            .Include(a => a.Employee)
            .Include(a => a.CurrentJob)
            .Include(a => a.NewJob)
            .Include(a => a.CreatedBy)
            .Include(j => j.OrderForChangeOfPosition);

        var searchTermQuery = GetSearchTerm(searchTerm);
        if (!string.IsNullOrWhiteSpace(searchTermQuery))
        {
            query = query.Where(p => p.SearchVector.Matches(EF.Functions.ToTsQuery(searchTermQuery)));
        }

        query = ApplyPages(query, page, pageSize);
        return await query.ToListAsync(cancellationToken);
    }
}
