using MTK.Modules.Hr.Domain.CalendarDays;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Services;
using Xunit;

namespace MTK.Tests.Hr;

public sealed class CalendarDayTests
{
    [Fact]
    public void Create_TrimsNameAndSetsYear()
    {
        var date = new DateOnly(2026, 1, 1);

        var result = CalendarDay.Create("  New Year  ", date, CalendarDayType.Holiday);

        Assert.True(result.IsSuccess);
        Assert.Equal("New Year", result.Value.Name);
        Assert.Equal(2026, result.Value.Year);
    }

    [Fact]
    public void Create_WithBlankName_ReturnsFailure()
    {
        var result = CalendarDay.Create(" ", new DateOnly(2026, 1, 1), CalendarDayType.Holiday);

        Assert.True(result.IsFailure);
        Assert.Equal("CalendarDay.EmptyName", result.Error.Code);
    }

    [Fact]
    public void WorkingDayCalculator_UsesFiveDayAndSixDayWeekendRules()
    {
        var calculator = new WorkingDayCalculatorService();
        var saturday = new DateOnly(2026, 10, 17);

        Assert.False(calculator.IsWorkingDay(saturday, WorkingDays.FiveDays, null));
        Assert.True(calculator.IsWorkingDay(saturday, WorkingDays.SixDays, null));
    }
}
