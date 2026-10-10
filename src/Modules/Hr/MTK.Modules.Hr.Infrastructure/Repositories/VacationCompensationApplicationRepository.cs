using MTK.Common.Domain.Abstractions;
using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.VacationCompensationApplications;
using Microsoft.EntityFrameworkCore;

using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class VacationCompensationApplicationRepository
    : Repository<VacationCompensationApplication>, IVacationCompensationApplicationRepository
{
    public VacationCompensationApplicationRepository(HrDbContext context) : base(context) { }

    public override async Task<List<VacationCompensationApplication>> SearchAsync(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria,
        string? searchTerm,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFiltersAndSort(filters, sortCriteria);
        query = ApplyPages(query, page, pageSize);

        // Include Employee, Company və VacationBalance
        query = query
            .Include(v => v.Employee)
            .Include(j => j.CompensationOrder);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<VacationCompensationApplication?> GetByIdWithLinesAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<VacationCompensationApplication>()
            .Include(v => v.Employee)
                .ThenInclude(e => e.Job)
            .Include(v => v.CreatedBy)
            .Include(v => v.CompensationOrder)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<List<VacationCompensationApplication>> GetByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<VacationCompensationApplication>()
            .Where(v => v.EmployeeId == employeeId)
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}