using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.Warnings;
using Microsoft.EntityFrameworkCore;

using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class WarningRepository : SearchableRepository<WarningOrder>, IWarningRepository
{
    public WarningRepository(HrDbContext context) : base(context)
    {
    }

    public override async Task<WarningOrder?> GetByIdDefaultAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<WarningOrder>()
            .Include(w => w.Employee)
            .Include(w => w.OrderExecutionSupervisor)
            .Include(w => w.CreatedBy)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task<WarningOrder?> GetByIdForExportAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<WarningOrder>()
            .Include(w => w.Employee)
            .Include(w => w.Employee)
                .ThenInclude(e => e.Job)
            .Include(w => w.OrderExecutionSupervisor)
            .Include(w => w.OrderExecutionSupervisor)
                .ThenInclude(e => e.Job)
            .Include(w => w.CreatedBy)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public override async Task<List<WarningOrder>> SearchAsync(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria,
        string? searchTerm,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<WarningOrder> query = ApplyFiltersAndSort(filters, sortCriteria)
            .Include(w => w.Employee)
            .Include(w => w.OrderExecutionSupervisor);

        var searchTermQuery = GetSearchTerm(searchTerm);
        if (!string.IsNullOrWhiteSpace(searchTermQuery))
        {
            query = query.Where(p => p.SearchVector.Matches(EF.Functions.ToTsQuery(searchTermQuery)));
        }

        query = ApplyPages(query, page, pageSize);
        return await query.ToListAsync(cancellationToken);
    }
}
