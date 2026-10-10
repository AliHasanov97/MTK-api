using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.EmployeeEducationHistories;
using Microsoft.EntityFrameworkCore;

using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class EmployeeEducationHistoryRepository
    : Repository<EmployeeEducationHistory>, IEmployeeEducationHistoryRepository
{
    public EmployeeEducationHistoryRepository(HrDbContext context) : base(context) { }

    public async Task<List<EmployeeEducationHistory>> GetByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<EmployeeEducationHistory>()
            .Include(e => e.EducationalInstitution)
            .Where(e => e.EmployeeId == employeeId)
            .OrderByDescending(e => e.EndDate)
            .ThenByDescending(e => e.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<EmployeeEducationHistory?> GetLatestByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<EmployeeEducationHistory>()
            .Where(e => e.EmployeeId == employeeId)
            .OrderByDescending(e => e.EndDate)
            .ThenByDescending(e => e.StartDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<(List<EmployeeEducationHistory> Items, int TotalCount)> SearchByEmployeeIdAsync(
        Guid employeeId,
        string? searchTerm,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Set<EmployeeEducationHistory>()
            .Include(e => e.EducationalInstitution)
            .Where(e => e.EmployeeId == employeeId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(e =>
                e.Faculty.ToLower().Contains(term) ||
                (e.Specialty != null && e.Specialty.ToLower().Contains(term)) ||
                (e.DiplomaNumber != null && e.DiplomaNumber.ToLower().Contains(term)) ||
                (e.RegisterNumber != null && e.RegisterNumber.ToLower().Contains(term)) ||
                (e.EducationalInstitution != null && e.EducationalInstitution.Name.ToLower().Contains(term)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(e => e.EndDate)
            .ThenByDescending(e => e.StartDate)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public override async Task<EmployeeEducationHistory?> GetByIdDefaultAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<EmployeeEducationHistory>()
            .Include(e => e.EducationalInstitution)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
}
