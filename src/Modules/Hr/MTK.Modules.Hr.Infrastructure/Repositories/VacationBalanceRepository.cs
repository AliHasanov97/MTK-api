using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.VacationBalances;
using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class VacationBalanceRepository : Repository<VacationBalance>, IVacationBalanceRepository
{
    public VacationBalanceRepository(HrDbContext context) : base(context) { }

    public async Task<VacationBalance?> GetCurrentAsync(Guid employeeId, DateOnly asOfDate, CancellationToken cancellationToken = default)
    {
        return await Context.Set<VacationBalance>()
            .FirstOrDefaultAsync(
                b => b.EmployeeId == employeeId && b.WorkYearStart <= asOfDate && asOfDate <= b.WorkYearEnd,
                cancellationToken);
    }

    public async Task<VacationBalance?> GetMostRecentBeforeAsync(Guid employeeId, DateOnly workYearStart, CancellationToken cancellationToken = default)
    {
        return await Context.Set<VacationBalance>()
            .Where(b => b.EmployeeId == employeeId && b.WorkYearStart < workYearStart)
            .OrderByDescending(b => b.WorkYearStart)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<VacationBalance?> GetOldestWithRemainingDaysAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<VacationBalance>()
            .Where(b => b.EmployeeId == employeeId && b.RemainingDays > 0)
            .OrderBy(b => b.WorkYearStart)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
