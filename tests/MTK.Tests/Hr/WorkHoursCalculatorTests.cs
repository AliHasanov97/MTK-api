using MTK.Modules.Hr.Domain.CalendarDays;
using MTK.Modules.Hr.Domain.EmployeeWorkSchedules;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.JobApplications;
using MTK.Modules.Hr.Domain.Services;
using Xunit;

namespace MTK.Tests.Hr;

public sealed class WorkHoursCalculatorTests
{
    private readonly WorkHoursCalculatorService _calculator = new();

    [Fact]
    public void CalculateNormHours_WithoutSchedule_UsesEightHoursOnWeekdays()
    {
        var employee = CreateEmployee(WorkingDays.FiveDays);

        var hours = _calculator.CalculateNormHours(
            employee,
            new DateOnly(2026, 10, 19), // Monday
            new DateOnly(2026, 10, 25), // Sunday
            []);

        Assert.Equal(40m, hours);
    }

    [Fact]
    public void CalculateNormHours_UsesScheduleAndExcludesHoliday()
    {
        var employee = CreateEmployee(WorkingDays.FiveDays);
        employee.WorkSchedules.Add(EmployeeWorkSchedule.Create(
            employee.Id,
            new DateOnly(2026, 1, 1),
            monday: 7m,
            tuesday: 7m,
            wednesday: 7m,
            thursday: 7m,
            friday: 7m));
        var holiday = CalendarDay.Create(
            "Holiday", new DateOnly(2026, 10, 21), CalendarDayType.Holiday).Value;

        var hours = _calculator.CalculateNormHours(
            employee,
            new DateOnly(2026, 10, 19),
            new DateOnly(2026, 10, 23),
            [holiday]);

        Assert.Equal(28m, hours);
    }

    [Fact]
    public void CalculateNormHours_WhenEndBeforeStart_ReturnsZero()
    {
        var hours = _calculator.CalculateNormHours(
            CreateEmployee(WorkingDays.FiveDays),
            new DateOnly(2026, 10, 20),
            new DateOnly(2026, 10, 19),
            []);

        Assert.Equal(0m, hours);
    }

    private static Employee CreateEmployee(WorkingDays workingDays) => Employee.Create(
        1,
        "Ali",
        "Mammadov",
        "Rashad",
        Gender.Male,
        new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero),
        Guid.NewGuid(),
        Guid.NewGuid(),
        workingDays: workingDays);
}
