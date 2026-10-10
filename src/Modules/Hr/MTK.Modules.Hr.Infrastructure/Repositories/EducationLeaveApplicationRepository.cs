using MTK.Common.Domain.Abstractions;
using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.EducationLeaveApplications;
using Microsoft.EntityFrameworkCore;

using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class EducationLeaveApplicationRepository
    : Repository<EducationLeaveApplication>, IEducationLeaveApplicationRepository
{
    public EducationLeaveApplicationRepository(HrDbContext context) : base(context) { }

    public override async Task<List<EducationLeaveApplication>> SearchAsync(
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
            .Include(e => e.Employee)
            .Include(e => e.CreatedBy)
            .Include(e => e.EducationLeaveOrder);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<EducationLeaveApplication?> GetByIdWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<EducationLeaveApplication>()
            .Include(e => e.Employee)
                .ThenInclude(emp => emp.Job)
            .Include(e => e.CreatedBy)
            .Include(e => e.EducationLeaveOrder)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<List<EducationLeaveApplication>> GetByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<EducationLeaveApplication>()
            .Where(e => e.EmployeeId == employeeId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<EducationLeaveApplication?> GetByIdWithLinesAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<EducationLeaveApplication>()
            .Include(e => e.Employee)
                .ThenInclude(emp => emp.Job)
            .Include(e => e.CreatedBy)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
}