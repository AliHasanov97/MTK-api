namespace MTK.Modules.Hr.Application.CalendarDays.GetCalendarDaysByYear;

/// <summary>
/// İl üzrə təqvim günləri cavabı
/// </summary>
public sealed record GetCalendarDaysByYearResponse
{
    /// <summary>
    /// İl
    /// </summary>
    public int Year { get; init; }

    /// <summary>
    /// Aylıq statistika
    /// </summary>
    public List<MonthSummary> Months { get; init; } = [];

    /// <summary>
    /// İllik cəmi
    /// </summary>
    public YearSummary YearTotal { get; init; } = new();
}

/// <summary>
/// Aylıq statistika
/// </summary>
public sealed record MonthSummary
{
    /// <summary>
    /// Ay (1-12)
    /// </summary>
    public int Month { get; init; }

    /// <summary>
    /// Ay adı
    /// </summary>
    public string MonthName { get; init; } = string.Empty;

    /// <summary>
    /// Təqvim günü sayı (ayda neçə gün var)
    /// </summary>
    public int TotalDays { get; init; }

    /// <summary>
    /// İş günü sayı
    /// </summary>
    public int WorkingDays { get; init; }

    /// <summary>
    /// Qeyri-iş günü sayı (həftə sonları + bayramlar)
    /// </summary>
    public int NonWorkingDays { get; init; }

    /// <summary>
    /// Qeyri-iş günləri (bayramlar və xüsusi günlər)
    /// </summary>
    public List<CalendarDayDto> NonWorkingDaysList { get; init; } = [];

    /// <summary>
    /// Ayın bütün təqvim qeydləri (bayram, qeyri-iş günü və əvəz iş günləri)
    /// </summary>
    public List<CalendarDayDto> Days { get; init; } = [];
}

/// <summary>
/// İllik cəmi statistika
/// </summary>
public sealed record YearSummary
{
    /// <summary>
    /// Təqvim günü sayı (ildə neçə gün var)
    /// </summary>
    public int TotalDays { get; init; }

    /// <summary>
    /// İş günü sayı
    /// </summary>
    public int WorkingDays { get; init; }

    /// <summary>
    /// Qeyri-iş günü sayı
    /// </summary>
    public int NonWorkingDays { get; init; }
}
