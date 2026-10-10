using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.WorkOnNonWorkdayOrders;
using Microsoft.EntityFrameworkCore;

using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class WorkOnNonWorkdayOrderRepository : SearchableRepository<WorkOnNonWorkdayOrder>, IWorkOnNonWorkdayOrderRepository
{
    public WorkOnNonWorkdayOrderRepository(HrDbContext context) : base(context)
    {
    }

    public override async Task<WorkOnNonWorkdayOrder?> GetByIdDefaultAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<WorkOnNonWorkdayOrder>()
            .Include(w => w.CreatedBy)
            .Include(w => w.FileAttachments)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public override async Task<List<WorkOnNonWorkdayOrder>> SearchAsync(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria,
        string? searchTerm,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<WorkOnNonWorkdayOrder> query = ApplyFiltersAndSort(filters, sortCriteria)
            .Include(w => w.CreatedBy);

        var searchTermQuery = GetSearchTerm(searchTerm);
        if (!string.IsNullOrWhiteSpace(searchTermQuery))
        {
            query = query.Where(p => p.SearchVector.Matches(EF.Functions.ToTsQuery(searchTermQuery)));
        }

        query = ApplyPages(query, page, pageSize);
        return await query.ToListAsync(cancellationToken);
    }
}
