using MTK.Modules.Hr.Domain.CalendarDays;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Services;
using Xunit;

namespace MTK.Tests.Hr;

public sealed class WorkingDayCalculatorTests
{
    private readonly WorkingDayCalculatorService _calculator = new();

    [Fact]
    public void IsHoliday_IgnoresHolidayThatDoesNotApplyToEmployeeSchedule()
    {
        var holiday = CalendarDay.Create(
            "Replacement holiday",
            new DateOnly(2026, 10, 16),
            CalendarDayType.Holiday,
            applicableWorkingDays: WorkingDays.FiveDays).Value;

        Assert.False(_calculator.IsHoliday(holiday, WorkingDays.SixDays));
        Assert.True(_calculator.IsHoliday(holiday, WorkingDays.FiveDays));
    }

    [Fact]
    public void CalculateVacationEndDate_SkipsHolidaysButCountsWeekends()
    {
        var startDate = new DateOnly(2026, 10, 16); // Friday
        var holiday = CalendarDay.Create(
            "Holiday", startDate, CalendarDayType.Holiday).Value;

        var endDate = _calculator.CalculateVacationEndDate(
            startDate,
            2,
            WorkingDays.FiveDays,
            [holiday]);

        // Friday is skipped as a holiday; Saturday and Sunday count as vacation days.
        Assert.Equal(new DateOnly(2026, 10, 18), endDate);
    }

    [Fact]
    public void CountVacationDays_IncludesWeekendsAndExcludesHolidays()
    {
        var startDate = new DateOnly(2026, 10, 16); // Friday
        var endDate = new DateOnly(2026, 10, 19); // Monday
        var holiday = CalendarDay.Create(
            "Holiday", new DateOnly(2026, 10, 18), CalendarDayType.Holiday).Value;

        var count = _calculator.CountVacationDays(
            startDate,
            endDate,
            WorkingDays.FiveDays,
            [holiday]);

        Assert.Equal(3, count);
    }

    [Fact]
    public void CountVacationDays_WhenEndPrecedesStart_ReturnsZero()
    {
        var count = _calculator.CountVacationDays(
            new DateOnly(2026, 10, 20),
            new DateOnly(2026, 10, 19),
            WorkingDays.FiveDays,
            []);

        Assert.Equal(0, count);
    }

    [Fact]
    public void CalculateReturnToWorkDate_SkipsWeekend()
    {
        var returnDate = _calculator.CalculateReturnToWorkDate(
            new DateOnly(2026, 10, 17), // Saturday
            WorkingDays.FiveDays,
            []);

        Assert.Equal(new DateOnly(2026, 10, 19), returnDate);
    }
}
