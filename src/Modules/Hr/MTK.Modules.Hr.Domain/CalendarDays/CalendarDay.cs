using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.Employees;

namespace MTK.Modules.Hr.Domain.CalendarDays;

/// <summary>
/// Təqvim günü - bayramlar və xüsusi günlər üçün vahid entity
/// </summary>
public sealed class CalendarDay : SearchableEntity
{
    private CalendarDay() { }

    /// <summary>
    /// Gün adı (Novruz, "8 Mart əvəzi", və s.)
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Tarix
    /// </summary>
    public DateOnly Date { get; private set; }

    /// <summary>
    /// İl
    /// </summary>
    public int Year { get; private set; }

    /// <summary>
    /// Gün növü (bayram, iş günü, qeyri-iş günü)
    /// </summary>
    public CalendarDayType DayType { get; private set; }

    /// <summary>
    /// Səbəb (nullable - məsələn: "Bayram günü əvəzinə şənbə iş günü")
    /// </summary>
    public string? Reason { get; private set; }

    /// <summary>
    /// Hansı iş qrafikinə tətbiq olunur (nullable = hamıya tətbiq olunur)
    /// Əvəzləmə bayramları üçün istifadə olunur:
    /// - null: Bütün işçilərə tətbiq olunur (əsas bayramlar)
    /// - FiveDays: Yalnız 5 günlük qrafikli işçilərə tətbiq olunur
    /// - SixDays: Yalnız 6 günlük qrafikli işçilərə tətbiq olunur
    /// </summary>
    public WorkingDays? ApplicableWorkingDays { get; private set; }

    /// <summary>
    /// Manual təqvim günü yaradır
    /// </summary>
    public static Result<CalendarDay> Create(
        string name,
        DateOnly date,
        CalendarDayType dayType,
        string? reason = null,
        WorkingDays? applicableWorkingDays = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<CalendarDay>(
                new Error("CalendarDay.EmptyName", "Adı boş ola bilməz"));

        var id = Guid.NewGuid();
        var calendarDay = new CalendarDay
        {
            Id = id,
            Name = name.Trim(),
            Date = date,
            Year = date.Year,
            DayType = dayType,
            Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim(),
            ApplicableWorkingDays = applicableWorkingDays
        };

        return Result.Success(calendarDay);
    }

    /// <summary>
    /// Təqvim gününü yenilə
    /// </summary>
    public Result Update(
        string name,
        DateOnly date,
        CalendarDayType dayType,
        string? reason = null,
        WorkingDays? applicableWorkingDays = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(
                new Error("CalendarDay.EmptyName", "Adı boş ola bilməz"));

        Name = name.Trim();
        Date = date;
        Year = date.Year;
        DayType = dayType;
        Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
        ApplicableWorkingDays = applicableWorkingDays;

        return Result.Success();
    }
}
