using MTK.Common.Domain.Abstractions;
using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.VacationApplications;
using Microsoft.EntityFrameworkCore;

using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class VacationApplicationRepository
    : Repository<VacationApplication>, IVacationApplicationRepository
{
    public VacationApplicationRepository(HrDbContext context) : base(context) { }

    public override async Task<List<VacationApplication>> SearchAsync(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria,
        string? searchTerm,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFiltersAndSort(filters, sortCriteria);
        query = ApplyPages(query, page, pageSize);

        // Include Employee, Company, CreatedBy və VacationBalances (junction table)
        query = query
            .Include(v => v.Employee)
            .Include(v => v.CreatedBy)
            .Include(j => j.VacationOrder);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<VacationApplication?> GetByIdWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<VacationApplication>()
            .Include(v => v.Employee)
                .ThenInclude(e => e.Job)
            .Include(v => v.CreatedBy)
            .Include(v => v.VacationOrder)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<List<VacationApplication>> GetByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<VacationApplication>()
            .Where(v => v.EmployeeId == employeeId)
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
