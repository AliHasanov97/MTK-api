using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.CalendarDays;
using MTK.Modules.Hr.Domain.Employees;
using Microsoft.EntityFrameworkCore;

using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class CalendarDayRepository : Repository<CalendarDay>, ICalendarDayRepository
{
    public CalendarDayRepository(HrDbContext context) : base(context) { }

    public async Task<List<CalendarDay>> GetByYearAsync(
        int year,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<CalendarDay>()
            .Where(c => c.Year == year)
            .OrderBy(c => c.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByDateAsync(
        DateOnly date,
        WorkingDays? applicableWorkingDays = null,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Set<CalendarDay>()
            .Where(c => c.Date == date);

        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        // ApplicableWorkingDays konflikt yoxlaması:
        // - Yeni gün null-dursa (hamıya tətbiq): hər hansı mövcud gün ilə konflikt
        // - Yeni gün FiveDays/SixDays-dirsə: yalnız null və ya eyni qrafik ilə konflikt
        if (applicableWorkingDays.HasValue)
        {
            // Yalnız null və ya eyni qrafik ilə konflikt
            query = query.Where(c =>
                c.ApplicableWorkingDays == null ||
                c.ApplicableWorkingDays == applicableWorkingDays.Value);
        }
        // applicableWorkingDays null-dursa, bütün mövcud günlərlə konflikt (filtr yoxdur)

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<List<CalendarDay>> GetByDateRangeAsync(
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<CalendarDay>()
            .Where(c => c.Date >= startDate && c.Date <= endDate)
            .OrderBy(c => c.Date)
            .ToListAsync(cancellationToken);
    }
}
