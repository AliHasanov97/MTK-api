using MTK.Modules.Hr.Domain.CalendarDays;
using MTK.Modules.Hr.Domain.Employees;

namespace MTK.Modules.Hr.Domain.Services;

/// <summary>
/// İş günü hesablamaları üçün domain servis
/// Məzuniyyət, bayram və həftə sonu günlərini nəzərə alaraq iş günlərini müəyyən edir
/// </summary>
public class WorkingDayCalculatorService
{
    /// <summary>
    /// Verilmiş tarix bayramdır? (Holiday və ya NonWorkingDay)
    /// İşçinin iş qrafikini nəzərə alır - əvəzləmə bayramları fərqli ola bilər.
    /// </summary>
    /// <param name="calendarDay">CalendarDay entity (əgər həmin tarix üçün varsa)</param>
    /// <param name="workingDays">İşçinin iş həftəsi (5 və ya 6 günlük)</param>
    /// <returns>True - bayramdır, False - bayram deyil</returns>
    public bool IsHoliday(CalendarDay? calendarDay, WorkingDays workingDays)
    {
        if (calendarDay == null)
            return false;

        // Əgər CalendarDay konkret iş qrafikinə tətbiq olunursa, uyğunluğu yoxla
        // Məsələn: əvəzləmə bayramı yalnız 5 günlük işçilərə aiddirsə,
        // 6 günlük işçi üçün bu gün bayram sayılmamalıdır
        if (calendarDay.ApplicableWorkingDays != null &&
            calendarDay.ApplicableWorkingDays != workingDays)
        {
            return false;
        }

        // Holiday və NonWorkingDay bayram sayılır
        // WorkingDay isə bayram deyil (həftə sonu iş günü edilib)
        return calendarDay.DayType != CalendarDayType.WorkingDay;
    }

    /// <summary>
    /// Verilmiş tarix iş günüdürmü? (həftəsonları və bayramlar nəzərə alınır)
    /// İşə qayıtma tarixi hesablaması üçün istifadə olunur.
    /// </summary>
    /// <param name="date">Yoxlanılacaq tarix</param>
    /// <param name="workingDays">İşçinin iş həftəsi (5 və ya 6 günlük)</param>
    /// <param name="calendarDay">CalendarDay entity (əgər həmin tarix üçün varsa)</param>
    /// <returns>True - iş günüdür, False - qeyri-iş günüdür</returns>
    public bool IsWorkingDay(
        DateOnly date,
        WorkingDays workingDays,
        CalendarDay? calendarDay)
    {
        // 1. CalendarDay varsa və bu işçiyə tətbiq olunursa
        if (calendarDay != null && AppliesToWorkingDays(calendarDay, workingDays))
        {
            // Holiday və NonWorkingDay qeyri-iş günüdür
            // WorkingDay iş günüdür (həftə sonu da olsa)
            return calendarDay.DayType == CalendarDayType.WorkingDay;
        }

        // 2. CalendarDay yoxdursa və ya bu işçiyə tətbiq olunmursa, normal həftə məntiqi
        return date.DayOfWeek switch
        {
            DayOfWeek.Sunday => false, // Bazar həmişə qeyri-iş günüdür
            DayOfWeek.Saturday => workingDays == WorkingDays.SixDays, // Şənbə 6 günlük üçün iş günüdür
            _ => true // Digər günlər iş günüdür
        };
    }

    /// <summary>
    /// CalendarDay bu iş qrafikinə tətbiq olunurmu?
    /// </summary>
    private bool AppliesToWorkingDays(CalendarDay calendarDay, WorkingDays workingDays)
    {
        // ApplicableWorkingDays null-dursa, hamıya tətbiq olunur
        // Əks halda yalnız uyğun qrafikə tətbiq olunur
        return calendarDay.ApplicableWorkingDays == null ||
               calendarDay.ApplicableWorkingDays == workingDays;
    }

    /// <summary>
    /// Verilmiş gün sayına görə EndDate hesablayır (YALNIZ bayramları atlayır, həftəsonları sayılır)
    /// Məzuniyyət müddəti hesablaması üçün istifadə olunur.
    /// Əmək Məcəlləsi maddə 135: Bayram günləri məzuniyyət müddətinə daxil edilmir.
    /// </summary>
    /// <param name="startDate">Başlanğıc tarixi</param>
    /// <param name="daysCount">İstənilən gün sayı</param>
    /// <param name="workingDays">İşçinin iş həftəsi (5 və ya 6 günlük)</param>
    /// <param name="calendarDays">Tarix aralığındakı CalendarDay-lər (bayramlar)</param>
    /// <returns>Son gün tarixi (EndDate)</returns>
    public DateOnly CalculateVacationEndDate(
        DateOnly startDate,
        int daysCount,
        WorkingDays workingDays,
        List<CalendarDay> calendarDays)
    {
        if (daysCount <= 0)
            return startDate;

        var currentDate = startDate;
        int countedDays = 0;
        const int maxIterations = 365;
        var iteration = 0;

        while (countedDays < daysCount && iteration < maxIterations)
        {
            var calendarDay = calendarDays.FirstOrDefault(cd => cd.Date == currentDate);

            // Bayram deyilsə, say (həftəsonları da sayılır!)
            // İşçinin iş qrafikini nəzərə al - əvəzləmə bayramları fərqli ola bilər
            if (!IsHoliday(calendarDay, workingDays))
            {
                countedDays++;

                if (countedDays == daysCount)
                {
                    return currentDate;
                }
            }

            currentDate = currentDate.AddDays(1);
            iteration++;
        }

        return currentDate;
    }

    /// <summary>
    /// İki tarix arasındakı məzuniyyət günlərinin sayını hesablayır (YALNIZ bayramlar çıxılır)
    /// </summary>
    /// <param name="startDate">Başlanğıc tarixi</param>
    /// <param name="endDate">Son tarix</param>
    /// <param name="workingDays">İşçinin iş həftəsi (5 və ya 6 günlük)</param>
    /// <param name="calendarDays">Tarix aralığındakı CalendarDay-lər (bayramlar)</param>
    /// <returns>Məzuniyyət günlərinin sayı</returns>
    public int CountVacationDays(
        DateOnly startDate,
        DateOnly endDate,
        WorkingDays workingDays,
        List<CalendarDay> calendarDays)
    {
        if (endDate < startDate)
            return 0;

        int count = 0;
        var currentDate = startDate;

        while (currentDate <= endDate)
        {
            var calendarDay = calendarDays.FirstOrDefault(cd => cd.Date == currentDate);

            // Bayram deyilsə, say (həftəsonları da sayılır!)
            // İşçinin iş qrafikini nəzərə al - əvəzləmə bayramları fərqli ola bilər
            if (!IsHoliday(calendarDay, workingDays))
            {
                count++;
            }

            currentDate = currentDate.AddDays(1);
        }

        return count;
    }

    /// <summary>
    /// Məzuniyyət bitdikdən sonra işə qayıtma tarixini hesablayır
    /// </summary>
    /// <param name="endDate">Məzuniyyətin son günü</param>
    /// <param name="workingDays">İşçinin iş həftəsi</param>
    /// <param name="calendarDays">Tarix aralığındakı CalendarDay-lər</param>
    /// <returns>İlk iş günü tarixi</returns>
    public DateOnly CalculateReturnToWorkDate(
        DateOnly endDate,
        WorkingDays workingDays,
        List<CalendarDay> calendarDays)
    {
        var currentDate = endDate.AddDays(1); // Məzuniyyətin ertəsi günündən başla
        const int maxIterations = 30; // Təhlükəsizlik üçün - maksimum 30 gün irəli get
        var iteration = 0;

        // İlk iş gününü tap
        while (iteration < maxIterations)
        {
            // Həmin tarix üçün CalendarDay varsa tap
            var calendarDay = calendarDays.FirstOrDefault(cd => cd.Date == currentDate);

            // İş günüdürsə, qayıt
            if (IsWorkingDay(currentDate, workingDays, calendarDay))
            {
                return currentDate;
            }

            // Növbəti günə keç
            currentDate = currentDate.AddDays(1);
            iteration++;
        }

        // Təhlükəsizlik: maksimum iterasiya keçildisə, endDate + 1 qaytar
        return endDate.AddDays(1);
    }
}
