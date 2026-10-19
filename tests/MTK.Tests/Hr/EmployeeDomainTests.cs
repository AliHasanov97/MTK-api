using MTK.Modules.Hr.Domain.EmployeeWorkSchedules;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.JobApplications;
using Xunit;

namespace MTK.Tests.Hr;

public sealed class EmployeeDomainTests
{
    [Fact]
    public void Create_SetsDefaultsAndFullName()
    {
        var employee = Employee.Create(
            17, "Leyla", "Aliyeva", "Rauf", Gender.Female,
            new DateTimeOffset(2026, 1, 5, 0, 0, 0, TimeSpan.Zero),
            Guid.NewGuid(), Guid.NewGuid());

        Assert.Equal("Leyla Aliyeva", employee.FullName);
        Assert.Equal(21, employee.VacationDays);
        Assert.Equal(EmploymentType.FullTime, employee.EmploymentType);
        Assert.Equal(EmployeeStatus.Active, employee.IsActive);
    }

    [Fact]
    public void GetScheduleForDate_ChoosesLatestScheduleThatHasStarted()
    {
        var employee = Employee.Create(
            17, "Leyla", "Aliyeva", "Rauf", Gender.Female,
            new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero),
            Guid.NewGuid(), Guid.NewGuid());
        var earlier = EmployeeWorkSchedule.Create(employee.Id, new DateOnly(2025, 1, 1), monday: 8m);
        var later = EmployeeWorkSchedule.Create(employee.Id, new DateOnly(2026, 1, 1), monday: 6m);
        employee.WorkSchedules.Add(earlier);
        employee.WorkSchedules.Add(later);

        Assert.Equal(earlier, employee.GetScheduleForDate(new DateOnly(2025, 6, 1)));
        Assert.Equal(later, employee.GetScheduleForDate(new DateOnly(2026, 6, 1)));
        Assert.Null(employee.GetScheduleForDate(new DateOnly(2024, 12, 31)));
    }

    [Fact]
    public void CalculateTotalWorkExperience_UsesCurrentJobAndReferenceDate()
    {
        var employee = Employee.Create(
            17, "Leyla", "Aliyeva", "Rauf", Gender.Female,
            new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero),
            Guid.NewGuid(), Guid.NewGuid());

        var days = employee.CalculateTotalWorkExperienceInDays(
            new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero));

        Assert.Equal(366, days);
        Assert.Equal(1, employee.CalculateTotalWorkExperience(
            new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero)).Years);
    }
}
