using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.EmploymentStatusChangeApplications;
using Microsoft.EntityFrameworkCore;

using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class EmploymentStatusChangeApplicationRepository : SearchableRepository<EmploymentStatusChangeApplication>, IEmploymentStatusChangeApplicationRepository
{
    public EmploymentStatusChangeApplicationRepository(HrDbContext context) : base(context)
    {
    }

    public override async Task<EmploymentStatusChangeApplication?> GetByIdDefaultAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<EmploymentStatusChangeApplication>()
            .Include(a => a.Employee)
            .Include(a => a.OrderExecutionSupervisor)
            .Include(a => a.CreatedBy)
            .Include(a => a.EmploymentStatusChangeOrder)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<EmploymentStatusChangeApplication?> GetByIdForExportAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<EmploymentStatusChangeApplication>()
            .Include(a => a.Employee)
                .ThenInclude(e => e.Job)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public override async Task<List<EmploymentStatusChangeApplication>> SearchAsync(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria,
        string? searchTerm,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<EmploymentStatusChangeApplication> query = ApplyFiltersAndSort(filters, sortCriteria)
            .Include(a => a.Employee)
            .Include(a => a.OrderExecutionSupervisor)
            .Include(a => a.CreatedBy)
            .Include(a => a.EmploymentStatusChangeOrder);

        var searchTermQuery = GetSearchTerm(searchTerm);
        if (!string.IsNullOrWhiteSpace(searchTermQuery))
        {
            query = query.Where(p => p.SearchVector.Matches(EF.Functions.ToTsQuery(searchTermQuery)));
        }

        query = ApplyPages(query, page, pageSize);
        return await query.ToListAsync(cancellationToken);
    }
}