using MTK.Modules.Hr.Domain.EmployeeWorkSchedules;
using Xunit;

namespace MTK.Tests.Hr;

public sealed class EmployeeWorkScheduleTests
{
    [Fact]
    public void GetWorkHours_ReturnsConfiguredHoursAndNullForRestDay()
    {
        var schedule = EmployeeWorkSchedule.Create(
            Guid.NewGuid(),
            new DateOnly(2026, 1, 1),
            monday: 8m,
            tuesday: 6.5m);

        Assert.Equal(8m, schedule.GetWorkHours(DayOfWeek.Monday));
        Assert.Equal(6.5m, schedule.GetWorkHours(DayOfWeek.Tuesday));
        Assert.Null(schedule.GetWorkHours(DayOfWeek.Wednesday));
    }
}
