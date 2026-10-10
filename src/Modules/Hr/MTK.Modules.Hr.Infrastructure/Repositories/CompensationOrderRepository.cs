using MTK.Common.Domain.Abstractions;
using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.CompensationOrders;
using Microsoft.EntityFrameworkCore;

using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class CompensationOrderRepository
    : Repository<CompensationOrder>, ICompensationOrderRepository
{
    public CompensationOrderRepository(HrDbContext context) : base(context) { }

    public override async Task<CompensationOrder?> GetByIdDefaultAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<CompensationOrder>()
            .Include(o => o.Employee)
                .ThenInclude(e => e.Job)
            .Include(o => o.CreatedBy)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public override async Task<List<CompensationOrder>> SearchAsync(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria,
        string? searchTerm,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFiltersAndSort(filters, sortCriteria);
        query = ApplyPages(query, page, pageSize);

        // Include Employee, Company, VacationBalance və CreatedBy
        query = query
            .Include(o => o.Employee)
            .Include(o => o.CreatedBy);

        return await query.ToListAsync(cancellationToken);
    }
    
    public async Task<CompensationOrder?> GetByCompensationApplicationIdAsync(
        Guid compensationApplicationId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<CompensationOrder>()
            .FirstOrDefaultAsync(
                o => o.CompensationApplicationId == compensationApplicationId,
                cancellationToken);
    }

    public async Task<CompensationOrder?> GetByIdWithEmployeeAndCompanyAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<CompensationOrder>()
            .Include(o => o.Employee)
                .ThenInclude(e => e.Job)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }
}