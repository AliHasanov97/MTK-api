using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.EmploymentStatusChangeOrders;
using Microsoft.EntityFrameworkCore;

using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class EmploymentStatusChangeOrderRepository : SearchableRepository<EmploymentStatusChangeOrder>, IEmploymentStatusChangeOrderRepository
{
    public EmploymentStatusChangeOrderRepository(HrDbContext context) : base(context) { }

    public override async Task<EmploymentStatusChangeOrder?> GetByIdDefaultAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<EmploymentStatusChangeOrder>()
            .Include(o => o.EmploymentStatusChangeApplication)
                .ThenInclude(a => a!.Employee)
            .Include(o => o.Employee)
            .Include(o => o.OrderExecutionSupervisor)
            .Include(o => o.CreatedBy)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<EmploymentStatusChangeOrder?> GetByIdForExportAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<EmploymentStatusChangeOrder>()
            .Include(o => o.Employee)
            .Include(o => o.Employee)
                .ThenInclude(e => e.Job)
            .Include(o => o.OrderExecutionSupervisor)
            .Include(o => o.OrderExecutionSupervisor)
                .ThenInclude(e => e.Job)
            .Include(o => o.EmploymentStatusChangeApplication)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public override async Task<List<EmploymentStatusChangeOrder>> SearchAsync(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria,
        string? searchTerm,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<EmploymentStatusChangeOrder> query = ApplyFiltersAndSort(filters, sortCriteria)
            .Include(o => o.EmploymentStatusChangeApplication)
                .ThenInclude(a => a!.Employee)
            .Include(o => o.Employee)
            .Include(o => o.OrderExecutionSupervisor)
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