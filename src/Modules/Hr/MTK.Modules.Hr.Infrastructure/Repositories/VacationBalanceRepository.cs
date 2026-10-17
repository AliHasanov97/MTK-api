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
        // RemainingDays = TotalEntitledDays - UsedDays - CompensatedDays
        // TotalEntitledDays = BaseEntitledDays + ExperienceBonusDays + KarabakhWorkerDays + ChildrenUnder14Days + DisabledChildDays
        return await Context.Set<VacationBalance>()
            .Where(b => b.EmployeeId == employeeId
                && (b.BaseEntitledDays + b.ExperienceBonusDays + b.KarabakhWorkerDays + b.ChildrenUnder14Days + b.DisabledChildDays - b.UsedDays - b.CompensatedDays) > 0)
            .OrderBy(b => b.WorkYearStart)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<VacationBalance>> GetAllByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<VacationBalance>()
            .Where(b => b.EmployeeId == employeeId)
            .OrderBy(b => b.WorkYearStart)
            .ToListAsync(cancellationToken);
    }
}
