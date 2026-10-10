using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.OrdersForChangeOfPosition;
using Microsoft.EntityFrameworkCore;

using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class OrderForChangeOfPositionRepository : SearchableRepository<OrderForChangeOfPosition>, IOrderForChangeOfPositionRepository
{
    public OrderForChangeOfPositionRepository(HrDbContext context) : base(context) { }

    public override async Task<OrderForChangeOfPosition?> GetByIdDefaultAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<OrderForChangeOfPosition>()
            .Include(o => o.ApplicationForChangeOfPosition)
                .ThenInclude(a => a!.Employee)
            .Include(o => o.ApplicationForChangeOfPosition)
            .Include(o => o.ApplicationForChangeOfPosition)
                .ThenInclude(a => a!.CurrentJob)
            .Include(o => o.ApplicationForChangeOfPosition)
            .Include(o => o.ApplicationForChangeOfPosition)
                .ThenInclude(a => a!.NewJob)
            .Include(o => o.Employee)
            .Include(o => o.CreatedBy)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public override async Task<List<OrderForChangeOfPosition>> SearchAsync(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria,
        string? searchTerm,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<OrderForChangeOfPosition> query = ApplyFiltersAndSort(filters, sortCriteria)
            .Include(o => o.ApplicationForChangeOfPosition)
                .ThenInclude(a => a!.Employee)
            .Include(o => o.ApplicationForChangeOfPosition)
            .Include(o => o.ApplicationForChangeOfPosition)
                .ThenInclude(a => a!.CurrentJob)
            .Include(o => o.ApplicationForChangeOfPosition)
            .Include(o => o.ApplicationForChangeOfPosition)
                .ThenInclude(a => a!.NewJob)
            .Include(o => o.Employee)
            .Include(o => o.CreatedBy);

        var searchTermQuery = GetSearchTerm(searchTerm);
        if (!string.IsNullOrWhiteSpace(searchTermQuery))
        {
            query = query.Where(p => p.SearchVector.Matches(EF.Functions.ToTsQuery(searchTermQuery)));
        }

        query = ApplyPages(query, page, pageSize);
        return await query.ToListAsync(cancellationToken);
    }
}
