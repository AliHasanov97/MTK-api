using MTK.Common.Domain.Abstractions;
using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.EducationLeaveOrders;
using Microsoft.EntityFrameworkCore;

using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class EducationLeaveOrderRepository
    : Repository<EducationLeaveOrder>, IEducationLeaveOrderRepository
{
    public EducationLeaveOrderRepository(HrDbContext context) : base(context) { }

    public override async Task<EducationLeaveOrder?> GetByIdDefaultAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<EducationLeaveOrder>()
            .Include(o => o.Employee)
                .ThenInclude(e => e.Job)
            .Include(o => o.CreatedBy)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public override async Task<List<EducationLeaveOrder>> SearchAsync(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria,
        string? searchTerm,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFiltersAndSort(filters, sortCriteria);
        query = ApplyPages(query, page, pageSize);

        // Include Employee, Company və CreatedBy
        query = query
            .Include(o => o.Employee)
            .Include(o => o.CreatedBy);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<EducationLeaveOrder?> GetByEducationLeaveApplicationIdAsync(
        Guid educationLeaveApplicationId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<EducationLeaveOrder>()
            .FirstOrDefaultAsync(
                o => o.EducationLeaveApplicationId == educationLeaveApplicationId,
                cancellationToken);
    }

    public async Task<EducationLeaveOrder?> GetByIdWithEmployeeAndCompanyAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<EducationLeaveOrder>()
            .Include(o => o.Employee)
                .ThenInclude(e => e.Job)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }
}
