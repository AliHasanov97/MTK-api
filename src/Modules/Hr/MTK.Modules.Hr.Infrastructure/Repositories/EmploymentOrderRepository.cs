using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.EmploymentOrders;
using Microsoft.EntityFrameworkCore;

using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class EmploymentOrderRepository : SearchableRepository<EmploymentOrder>, IEmploymentOrderRepository
{
    public EmploymentOrderRepository(HrDbContext context) : base(context) { }

    public override async Task<EmploymentOrder?> GetByIdDefaultAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<EmploymentOrder>()
            .Include(e => e.JobApplication)
            .Include(e => e.JobApplication)
                .ThenInclude(ja => ja!.Job)
            .Include(e => e.CreatedBy)
            .Include(e => e.LaborCodeCase)
                .ThenInclude(l => l!.Parent)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public override async Task<List<EmploymentOrder>> SearchAsync(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria,
        string? searchTerm,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<EmploymentOrder> query = ApplyFiltersAndSort(filters, sortCriteria)
            .Include(e => e.JobApplication)
            .Include(e => e.JobApplication)
                .ThenInclude(ja => ja!.Job)
            .Include(e => e.CreatedBy)
            .Include(e => e.LaborCodeCase);

        var searchTermQuery = GetSearchTerm(searchTerm);
        if (!string.IsNullOrWhiteSpace(searchTermQuery))
        {
            query = query.Where(p => p.SearchVector.Matches(EF.Functions.ToTsQuery(searchTermQuery)));
        }

        query = ApplyPages(query, page, pageSize);
        return await query.ToListAsync(cancellationToken);
    }
}
