using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.BonusOrders;
using Microsoft.EntityFrameworkCore;

using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class BonusOrderRepository : SearchableRepository<BonusOrder>, IBonusOrderRepository
{
    public BonusOrderRepository(HrDbContext context) : base(context)
    {
    }

    public override async Task<BonusOrder?> GetByIdDefaultAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<BonusOrder>()
            .Include(bo => bo.Employee)
            .Include(bo => bo.OrderExecutionSupervisor)
            .Include(bo => bo.CreatedBy)
            .Include(bo => bo.FileAttachments)
            .FirstOrDefaultAsync(bo => bo.Id == id, cancellationToken);
    }

    public async Task<BonusOrder?> GetByIdForExportAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<BonusOrder>()
            .Include(bo => bo.Employee)
            .Include(bo => bo.Employee)
                .ThenInclude(e => e.Job)
            .Include(bo => bo.OrderExecutionSupervisor)
            .Include(bo => bo.OrderExecutionSupervisor)
                .ThenInclude(e => e.Job)
            .Include(bo => bo.CreatedBy)
            .FirstOrDefaultAsync(bo => bo.Id == id, cancellationToken);
    }

    public override async Task<List<BonusOrder>> SearchAsync(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria,
        string? searchTerm,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<BonusOrder> query = ApplyFiltersAndSort(filters, sortCriteria)
            .Include(bo => bo.Employee)
            .Include(bo => bo.OrderExecutionSupervisor)
            .Include(bo => bo.CreatedBy);

        var searchTermQuery = GetSearchTerm(searchTerm);
        if (!string.IsNullOrWhiteSpace(searchTermQuery))
        {
            query = query.Where(p => p.SearchVector.Matches(EF.Functions.ToTsQuery(searchTermQuery)));
        }

        query = ApplyPages(query, page, pageSize);
        return await query.ToListAsync(cancellationToken);
    }
}
