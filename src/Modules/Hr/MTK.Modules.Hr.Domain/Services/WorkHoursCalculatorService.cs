using MTK.Modules.Hr.Domain.CalendarDays;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.EmployeeWorkSchedules;

namespace MTK.Modules.Hr.Domain.Services;

/// <summary>
/// İşçinin qrafikinə (həftəlik iş saatları) və istehsalat təqviminə (bayramlar) görə
/// verilmiş tarix aralığındaki İŞ SAATI NORMASINI hesablayır. Bu, Timesheet-dəki
/// "aylıq iş saatı norma (NormWorkedHours)" ilə eyni məntiqdir — əmək haqqı düzəlişləri də
/// bunun üzərindən hesablanır: saatlıq dərəcə = baza maaş / ayın iş saatı norması.
/// </summary>
public class WorkHoursCalculatorService
{
    /// <summary>Qrafik olmadıqda bir iş günü üçün standart saat (ehtiyat dəyər).</summary>
    public const decimal DefaultDailyHours = 8m;

    /// <summary>
    /// Verilmiş aralıqda işçinin işləməli olduğu saatların cəmi (norma).
    /// Bayram/qeyri-iş günləri çıxılır, "iş günü edilmiş" günlər daxil edilir.
    /// </summary>
    public decimal CalculateNormHours(
        Employee employee,
        DateOnly startDate,
        DateOnly endDate,
        IReadOnlyCollection<CalendarDay> calendarDays)
    {
        if (endDate < startDate)
        {
            return 0m;
        }

        var workingDays = employee.WorkingDays ?? WorkingDays.FiveDays;
        var calendarDayMap = BuildCalendarDayMap(calendarDays, workingDays);

        decimal normHours = 0m;
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            normHours += GetDayHours(employee, workingDays, date, calendarDayMap);
        }

        return normHours;
    }

    private static Dictionary<DateOnly, CalendarDay> BuildCalendarDayMap(
        IReadOnlyCollection<CalendarDay> calendarDays,
        WorkingDays workingDays)
    {
        return calendarDays
            .Where(c => c.ApplicableWorkingDays == null || c.ApplicableWorkingDays == workingDays)
            .GroupBy(c => c.Date)
            .ToDictionary(
                g => g.Key,
                g => g.OrderByDescending(c => c.ApplicableWorkingDays.HasValue).First());
    }

    private decimal GetDayHours(
        Employee employee,
        WorkingDays workingDays,
        DateOnly date,
        Dictionary<DateOnly, CalendarDay> calendarDayMap)
    {
        calendarDayMap.TryGetValue(date, out var calendarDay);

        // Bayram / qeyri-iş günü — iş saatı yoxdur.
        if (calendarDay is { DayType: CalendarDayType.Holiday or CalendarDayType.NonWorkingDay })
        {
            return 0m;
        }

        // İşə başlamazdan əvvəlki günlərdə hələ qrafik qüvvədə olmadığı üçün işçinin ilk qrafiki götürülür.
        var schedule = employee.GetScheduleForDate(date)
                       ?? employee.WorkSchedules.OrderBy(s => s.EffectiveFrom).FirstOrDefault();

        var usableSchedule = schedule is not null && HasAnyHours(schedule);

        if (usableSchedule)
        {
            var hours = schedule!.GetWorkHours(date.DayOfWeek);
            if (hours is > 0)
            {
                return hours.Value;
            }

            // Qrafikə görə bu gün istirahətdir; yalnız "iş günü edilmiş" gündürsə default saat tətbiq olunur.
            return calendarDay is { DayType: CalendarDayType.WorkingDay } ? DefaultDailyHours : 0m;
        }

        // Qrafik yoxdur/boşdur (məs. hələ təyin olunmayıb) — standart iş həftəsinə (5/6 gün) görə 8 saat.
        if (calendarDay is { DayType: CalendarDayType.WorkingDay })
        {
            return DefaultDailyHours;
        }

        var isWeekend = date.DayOfWeek == DayOfWeek.Sunday ||
                        (date.DayOfWeek == DayOfWeek.Saturday && workingDays != WorkingDays.SixDays);

        return isWeekend ? 0m : DefaultDailyHours;
    }

    private static bool HasAnyHours(EmployeeWorkSchedule schedule)
    {
        return schedule.Monday is > 0 || schedule.Tuesday is > 0 || schedule.Wednesday is > 0 ||
               schedule.Thursday is > 0 || schedule.Friday is > 0 || schedule.Saturday is > 0 ||
               schedule.Sunday is > 0;
    }
}
