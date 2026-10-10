using MTK.Common.Domain.Abstractions;
using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.VacationOrders;
using Microsoft.EntityFrameworkCore;

using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class VacationOrderRepository
    : Repository<VacationOrder>, IVacationOrderRepository
{
    public VacationOrderRepository(HrDbContext context) : base(context) { }

    public override async Task<VacationOrder?> GetByIdDefaultAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<VacationOrder>()
            .Include(o => o.Employee)
                .ThenInclude(e => e.Job)
            .Include(o => o.CreatedBy)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public override async Task<List<VacationOrder>> SearchAsync(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria,
        string? searchTerm,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFiltersAndSort(filters, sortCriteria);
        query = ApplyPages(query, page, pageSize);

        // Include Employee, Company, CreatedBy
        query = query
            .Include(o => o.Employee)
            .Include(o => o.CreatedBy);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<VacationOrder?> GetByIdWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<VacationOrder>()
            .Include(o => o.Employee)
                .ThenInclude(e => e.Job)
            .Include(o => o.CreatedBy)
            .Include(o => o.VacationApplication)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<List<VacationOrder>> GetByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<VacationOrder>()
            .Where(o => o.EmployeeId == employeeId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<VacationOrder?> GetByVacationApplicationIdAsync(
        Guid vacationApplicationId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<VacationOrder>()
            .FirstOrDefaultAsync(
                o => o.VacationApplicationId == vacationApplicationId,
                cancellationToken);
    }
}
