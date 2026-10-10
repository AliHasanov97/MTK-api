using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.Employees;

namespace MTK.Modules.Hr.Domain.CalendarDays;

public interface ICalendarDayRepository : IRepository<CalendarDay>
{
    /// <summary>
    /// İl üzrə təqvim günlərini al
    /// </summary>
    Task<List<CalendarDay>> GetByYearAsync(int year, CancellationToken cancellationToken = default);

    /// <summary>
    /// Eyni tarixdə və eyni iş qrafiki üçün təqvim günü mövcuddur?
    /// ApplicableWorkingDays nəzərə alınır:
    /// - null (hamıya tətbiq): hər hansı mövcud gün ilə konflikt
    /// - FiveDays/SixDays: null və ya eyni qrafik ilə konflikt
    /// </summary>
    Task<bool> ExistsByDateAsync(
        DateOnly date,
        WorkingDays? applicableWorkingDays = null,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// Tarix aralığı üzrə təqvim günlərini al
    /// </summary>
    Task<List<CalendarDay>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);
}
