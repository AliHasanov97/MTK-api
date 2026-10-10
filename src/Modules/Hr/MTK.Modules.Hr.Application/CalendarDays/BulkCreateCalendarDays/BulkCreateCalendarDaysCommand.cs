using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Domain.CalendarDays;
using MTK.Modules.Hr.Domain.Employees;

namespace MTK.Modules.Hr.Application.CalendarDays.BulkCreateCalendarDays;

/// <summary>
/// Təqvim günlərini toplu əlavə edir. Artıq mövcud olan tarixlər atlanır.
/// </summary>
public sealed record BulkCreateCalendarDaysCommand : ICommand<BulkCreateCalendarDaysResponse>
{
    public List<BulkCalendarDayItem> Days { get; init; } = [];
}

public sealed record BulkCalendarDayItem
{
    public string Name { get; init; } = string.Empty;
    public DateOnly Date { get; init; }
    public CalendarDayType DayType { get; init; }
    public string? Reason { get; init; }

    /// <summary>null = hamıya, FiveDays / SixDays = yalnız həmin qrafikə</summary>
    public WorkingDays? ApplicableWorkingDays { get; init; }
}

public sealed record BulkCreateCalendarDaysResponse
{
    public int CreatedCount { get; init; }

    /// <summary>Artıq mövcud olduğu üçün əlavə edilməyən tarixlər</summary>
    public List<DateOnly> SkippedDates { get; init; } = [];
}
