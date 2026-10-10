using MTK.Common.Domain.Abstractions;
using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.VacationReturnApplications;
using Microsoft.EntityFrameworkCore;

using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class VacationReturnApplicationRepository
    : Repository<VacationReturnApplication>, IVacationReturnApplicationRepository
{
    public VacationReturnApplicationRepository(HrDbContext context) : base(context) { }

    public override async Task<List<VacationReturnApplication>> SearchAsync(
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
            .Include(v => v.CreatedBy)
            .Include(v => v.VacationReturnOrder);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<VacationReturnApplication?> GetByIdWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<VacationReturnApplication>()
            .Include(v => v.Employee)
                .ThenInclude(e => e.Job)
            .Include(v => v.CreatedBy)
            .Include(v => v.VacationReturnOrder)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<List<VacationReturnApplication>> GetByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<VacationReturnApplication>()
            .Where(v => v.EmployeeId == employeeId)
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
