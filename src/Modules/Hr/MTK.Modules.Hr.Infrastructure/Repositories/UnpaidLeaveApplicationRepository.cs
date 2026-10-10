using MTK.Common.Domain.Abstractions;
using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.UnpaidLeaveApplications;
using Microsoft.EntityFrameworkCore;

using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class UnpaidLeaveApplicationRepository
    : Repository<UnpaidLeaveApplication>, IUnpaidLeaveApplicationRepository
{
    public UnpaidLeaveApplicationRepository(HrDbContext context) : base(context) { }

    public override async Task<List<UnpaidLeaveApplication>> SearchAsync(
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
            .Include(u => u.Employee)
            .Include(u => u.CreatedBy)
            .Include(j => j.UnpaidLeaveOrder);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<UnpaidLeaveApplication?> GetByIdWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<UnpaidLeaveApplication>()
            .Include(u => u.Employee)
                .ThenInclude(e => e.Job)
            .Include(u => u.CreatedBy)
            .Include(u => u.UnpaidLeaveOrder)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<List<UnpaidLeaveApplication>> GetByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<UnpaidLeaveApplication>()
            .Where(u => u.EmployeeId == employeeId)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<UnpaidLeaveApplication?> GetByIdWithLinesAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<UnpaidLeaveApplication>()
            .Include(u => u.Employee)
                .ThenInclude(e => e.Job)
            .Include(u => u.CreatedBy)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }
}
