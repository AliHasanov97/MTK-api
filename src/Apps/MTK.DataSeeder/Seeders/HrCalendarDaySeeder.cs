using MediatR;
using Microsoft.Extensions.Logging;
using MTK.Modules.Hr.Application.CalendarDays.BulkCreateCalendarDays;
using MTK.Modules.Hr.Domain.CalendarDays;

namespace MTK.DataSeeder.Seeders;

/// <summary>
/// İstehsalat təqvimini (CalendarDays) seed edir — cari və növbəti il üçün Azərbaycanın SABİT
/// tarixli rəsmi bayramları. Hicri təqvimə görə hər il tarixi dəyişən bayramlar (Ramazan/Qurban)
/// BURAYA DAXİL EDİLMİR — real tarixlər hər il üçün əl ilə (Təqvim bölməsindən) əlavə edilməlidir.
/// </summary>
public sealed class HrCalendarDaySeeder
{
    private readonly ISender _sender;
    private readonly ILogger<HrCalendarDaySeeder> _logger;
    private readonly ICalendarDayRepository _calendarDayRepository;

    public HrCalendarDaySeeder(
        ISender sender,
        ILogger<HrCalendarDaySeeder> logger,
        ICalendarDayRepository calendarDayRepository)
    {
        _sender = sender;
        _logger = logger;
        _calendarDayRepository = calendarDayRepository;
    }

    private static IEnumerable<BulkCalendarDayItem> FixedHolidaysForYear(int year)
    {
        BulkCalendarDayItem Day(string name, int month, int day) => new()
        {
            Name = name,
            Date = new DateOnly(year, month, day),
            DayType = CalendarDayType.Holiday,
        };

        yield return Day("Yeni il bayramı", 1, 1);
        yield return Day("Yeni il bayramı", 1, 2);
        yield return Day("Beynəlxalq Qadınlar Günü", 3, 8);
        yield return Day("Novruz bayramı", 3, 20);
        yield return Day("Novruz bayramı", 3, 21);
        yield return Day("Novruz bayramı", 3, 22);
        yield return Day("Novruz bayramı", 3, 23);
        yield return Day("Novruz bayramı", 3, 24);
        yield return Day("Qələbə Günü", 5, 9);
        yield return Day("Respublika Günü", 5, 28);
        yield return Day("Milli Qurtuluş Günü", 6, 15);
        yield return Day("Zəfər Günü", 11, 8);
        yield return Day("Həmrəylik Günü", 12, 31);
    }

    public async Task SeedAsync()
    {
        var currentYear = DateTime.UtcNow.Year;
        var existing = await _calendarDayRepository.ListAsync();

        if (existing.Any(d => d.Year == currentYear || d.Year == currentYear + 1))
        {
            _logger.LogInformation("  ℹ️  {Year}/{NextYear} üçün təqvim günləri artıq mövcuddur", currentYear, currentYear + 1);
            return;
        }

        var days = FixedHolidaysForYear(currentYear)
            .Concat(FixedHolidaysForYear(currentYear + 1))
            .ToList();

        var result = await _sender.Send(new BulkCreateCalendarDaysCommand { Days = days });

        if (result.IsSuccess)
        {
            _logger.LogInformation(
                "  📊 {Created} təqvim günü yaradıldı ({Skipped} artıq mövcud olduğu üçün atlandı)",
                result.Value.CreatedCount, result.Value.SkippedDates.Count);
        }
        else
        {
            _logger.LogError("  ❌ Təqvim günləri yaradıla bilmədi: {Error}", result.Error);
        }
    }
}
