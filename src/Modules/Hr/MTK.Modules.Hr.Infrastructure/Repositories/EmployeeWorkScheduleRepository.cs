using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.EmployeeWorkSchedules;
using Microsoft.EntityFrameworkCore;

using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class EmployeeWorkScheduleRepository : Repository<EmployeeWorkSchedule>, IEmployeeWorkScheduleRepository
{
    public EmployeeWorkScheduleRepository(HrDbContext context) : base(context) { }

    public async Task<EmployeeWorkSchedule?> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        // Ən son (cari) qrafiki qaytarır
        return await Context.Set<EmployeeWorkSchedule>()
            .Include(s => s.Employee)
            .Where(s => s.EmployeeId == employeeId)
            .OrderByDescending(s => s.EffectiveFrom)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<EmployeeWorkSchedule?> GetByEmployeeAndDateAsync(
        Guid employeeId, DateOnly effectiveFrom, CancellationToken cancellationToken = default)
    {
        return await Context.Set<EmployeeWorkSchedule>()
            .FirstOrDefaultAsync(s => s.EmployeeId == employeeId && s.EffectiveFrom == effectiveFrom, cancellationToken);
    }

    public async Task<List<EmployeeWorkSchedule>> GetAllByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<EmployeeWorkSchedule>()
            .Where(s => s.EmployeeId == employeeId)
            .OrderByDescending(s => s.EffectiveFrom)
            .ToListAsync(cancellationToken);
    }
}
