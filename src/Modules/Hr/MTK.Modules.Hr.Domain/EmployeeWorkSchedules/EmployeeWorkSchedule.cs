using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.Employees;

namespace MTK.Modules.Hr.Domain.EmployeeWorkSchedules;

/// <summary>
/// İşçinin həftəlik iş qrafiki - hər işçi üçün tarixçə ilə çoxlu record
/// NULL = istirahət günü, dəyər = iş saatı
/// </summary>
public sealed class EmployeeWorkSchedule : Entity
{
    private EmployeeWorkSchedule() { }

    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = null!;

    /// <summary>
    /// Bu qrafikin qüvvəyə minmə tarixi
    /// </summary>
    public DateOnly EffectiveFrom { get; private set; }

    /// <summary>
    /// Bazar ertəsi iş saatı (NULL = istirahət)
    /// </summary>
    public decimal? Monday { get; private set; }

    /// <summary>
    /// Çərşənbə axşamı iş saatı (NULL = istirahət)
    /// </summary>
    public decimal? Tuesday { get; private set; }

    /// <summary>
    /// Çərşənbə iş saatı (NULL = istirahət)
    /// </summary>
    public decimal? Wednesday { get; private set; }

    /// <summary>
    /// Cümə axşamı iş saatı (NULL = istirahət)
    /// </summary>
    public decimal? Thursday { get; private set; }

    /// <summary>
    /// Cümə iş saatı (NULL = istirahət)
    /// </summary>
    public decimal? Friday { get; private set; }

    /// <summary>
    /// Şənbə iş saatı (NULL = istirahət)
    /// </summary>
    public decimal? Saturday { get; private set; }

    /// <summary>
    /// Bazar iş saatı (NULL = istirahət)
    /// </summary>
    public decimal? Sunday { get; private set; }

    public static EmployeeWorkSchedule Create(
        Guid employeeId,
        DateOnly effectiveFrom,
        decimal? monday = null,
        decimal? tuesday = null,
        decimal? wednesday = null,
        decimal? thursday = null,
        decimal? friday = null,
        decimal? saturday = null,
        decimal? sunday = null)
    {
        return new EmployeeWorkSchedule
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            EffectiveFrom = effectiveFrom,
            Monday = monday,
            Tuesday = tuesday,
            Wednesday = wednesday,
            Thursday = thursday,
            Friday = friday,
            Saturday = saturday,
            Sunday = sunday
        };
    }

    public void Update(
        decimal? monday,
        decimal? tuesday,
        decimal? wednesday,
        decimal? thursday,
        decimal? friday,
        decimal? saturday,
        decimal? sunday)
    {
        Monday = monday;
        Tuesday = tuesday;
        Wednesday = wednesday;
        Thursday = thursday;
        Friday = friday;
        Saturday = saturday;
        Sunday = sunday;
    }

    /// <summary>
    /// Həftənin günü üzrə iş saatını qaytarır
    /// </summary>
    public decimal? GetWorkHours(DayOfWeek dayOfWeek)
    {
        return dayOfWeek switch
        {
            DayOfWeek.Monday => Monday,
            DayOfWeek.Tuesday => Tuesday,
            DayOfWeek.Wednesday => Wednesday,
            DayOfWeek.Thursday => Thursday,
            DayOfWeek.Friday => Friday,
            DayOfWeek.Saturday => Saturday,
            DayOfWeek.Sunday => Sunday,
            _ => null
        };
    }
}
