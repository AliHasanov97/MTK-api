using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Domain.CalendarDays;
using MTK.Modules.Hr.Domain.Employees;

namespace MTK.Modules.Hr.Application.CalendarDays.UpdateCalendarDay;

public sealed record UpdateCalendarDayCommand : ICommand<CalendarDayDto>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateOnly Date { get; init; }
    public CalendarDayType DayType { get; init; }
    public string? Reason { get; init; }

    /// <summary>
    /// Hansı iş qrafikinə tətbiq olunur (null = hamıya tətbiq olunur)
    /// </summary>
    public WorkingDays? ApplicableWorkingDays { get; init; }
}
