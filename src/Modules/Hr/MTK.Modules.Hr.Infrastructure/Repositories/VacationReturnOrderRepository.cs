using MTK.Common.Domain.Abstractions;
using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.VacationReturnOrders;
using Microsoft.EntityFrameworkCore;

using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class VacationReturnOrderRepository
    : Repository<VacationReturnOrder>, IVacationReturnOrderRepository
{
    public VacationReturnOrderRepository(HrDbContext context) : base(context) { }

    public override async Task<List<VacationReturnOrder>> SearchAsync(
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
            .Include(v => v.Employee)
            .Include(v => v.CreatedBy);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<VacationReturnOrder?> GetByIdWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<VacationReturnOrder>()
            .Include(v => v.Employee)
                .ThenInclude(e => e.Job)
            .Include(v => v.CreatedBy)
            .Include(v => v.VacationReturnApplication)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<List<VacationReturnOrder>> GetByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<VacationReturnOrder>()
            .Where(v => v.EmployeeId == employeeId)
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
