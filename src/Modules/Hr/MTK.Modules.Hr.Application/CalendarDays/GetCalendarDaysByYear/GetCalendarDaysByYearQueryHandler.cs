using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.CalendarDays;
using MTK.Modules.Hr.Domain.Employees;
using System.Globalization;

namespace MTK.Modules.Hr.Application.CalendarDays.GetCalendarDaysByYear;

internal sealed class GetCalendarDaysByYearQueryHandler : IQueryHandler<GetCalendarDaysByYearQuery, GetCalendarDaysByYearResponse>
{
    private readonly ICalendarDayRepository _repository;
    private readonly IMapper _mapper;

    // Azərbaycan dilində ay adları
    private static readonly string[] AzMonthNames =
    [
        "", "Yanvar", "Fevral", "Mart", "Aprel", "May", "İyun",
        "İyul", "Avqust", "Sentyabr", "Oktyabr", "Noyabr", "Dekabr"
    ];

    public GetCalendarDaysByYearQueryHandler(
        ICalendarDayRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<GetCalendarDaysByYearResponse>> Handle(
        GetCalendarDaysByYearQuery query,
        CancellationToken cancellationToken)
    {
        var calendarDays = await _repository.GetByYearAsync(query.Year, cancellationToken);

        // WorkingDays filteri verilibsə, yalnız həmin qrafikə aid günləri qaytar
        var workingDays = query.WorkingDays ?? WorkingDays.FiveDays;

        var filteredCalendarDays = calendarDays
            .Where(cd => cd.ApplicableWorkingDays == null ||
                         cd.ApplicableWorkingDays == workingDays)
            .ToList();

        // Aylıq statistika hesabla
        var monthSummaries = new List<MonthSummary>();

        for (int month = 1; month <= 12; month++)
        {
            var summary = CalculateMonthSummary(query.Year, month, workingDays, filteredCalendarDays);
            monthSummaries.Add(summary);
        }

        // İllik cəmi
        var yearTotal = new YearSummary
        {
            TotalDays = monthSummaries.Sum(m => m.TotalDays),
            WorkingDays = monthSummaries.Sum(m => m.WorkingDays),
            NonWorkingDays = monthSummaries.Sum(m => m.NonWorkingDays)
        };

        var response = new GetCalendarDaysByYearResponse
        {
            Year = query.Year,
            Months = monthSummaries,
            YearTotal = yearTotal
        };

        return Result.Success(response);
    }

    private MonthSummary CalculateMonthSummary(
        int year,
        int month,
        WorkingDays workingDays,
        List<CalendarDay> calendarDays)
    {
        var daysInMonth = DateTime.DaysInMonth(year, month);
        var monthCalendarDays = calendarDays.Where(cd => cd.Date.Month == month).ToList();

        int workingDaysCount = 0;
        int nonWorkingDaysCount = 0;

        for (int day = 1; day <= daysInMonth; day++)
        {
            var date = new DateOnly(year, month, day);
            var calendarDay = monthCalendarDays.FirstOrDefault(cd => cd.Date == date);

            bool isWorkingDay = IsWorkingDay(date, workingDays, calendarDay);

            if (isWorkingDay)
            {
                workingDaysCount++;
            }
            else
            {
                nonWorkingDaysCount++;
            }
        }

        // Qeyri-iş günləri (bayramlar) - CalendarDay ilə qeyd olunmuş
        var nonWorkingDaysList = monthCalendarDays
            .Where(cd => cd.DayType == CalendarDayType.Holiday ||
                         cd.DayType == CalendarDayType.NonWorkingDay)
            .Select(cd => _mapper.Map<CalendarDayDto>(cd))
            .ToList();

        return new MonthSummary
        {
            Month = month,
            MonthName = AzMonthNames[month],
            TotalDays = daysInMonth,
            WorkingDays = workingDaysCount,
            NonWorkingDays = nonWorkingDaysCount,
            NonWorkingDaysList = nonWorkingDaysList,
            Days = monthCalendarDays.OrderBy(cd => cd.Date).Select(cd => _mapper.Map<CalendarDayDto>(cd)).ToList()
        };
    }

    private bool IsWorkingDay(DateOnly date, WorkingDays workingDays, CalendarDay? calendarDay)
    {
        // CalendarDay varsa və bu işçiyə tətbiq olunursa
        if (calendarDay != null)
        {
            // WorkingDay iş günüdür, digərləri (Holiday, NonWorkingDay) deyil
            return calendarDay.DayType == CalendarDayType.WorkingDay;
        }

        // Normal həftə məntiqi
        return date.DayOfWeek switch
        {
            DayOfWeek.Sunday => false,
            DayOfWeek.Saturday => workingDays == WorkingDays.SixDays,
            _ => true
        };
    }
}
