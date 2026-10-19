using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.JobApplications;
using MTK.Modules.Hr.Domain.VacationBalances;
using Xunit;

namespace MTK.Tests.Hr;

public sealed class VacationEntitlementCalculatorTests
{
    [Fact]
    public void Calculate_AddsExperienceFamilyAndLocationBonuses()
    {
        var employee = CreateEmployee(
            new DateTimeOffset(2010, 1, 1, 0, 0, 0, TimeSpan.Zero),
            Gender.Female,
            childrenUnder14: 3,
            karabakhWorker: true,
            disabledChild: true);

        var breakdown = VacationEntitlementCalculator.Calculate(
            employee,
            new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero));

        Assert.Equal(6, breakdown.ExperienceBonusDays);
        Assert.Equal(5, breakdown.ChildrenUnder14Days);
        Assert.Equal(5, breakdown.KarabakhWorkerDays);
        Assert.Equal(5, breakdown.DisabledChildDays);
        Assert.Equal(21, breakdown.TotalAdditionalDays);
    }

    [Fact]
    public void Calculate_DoesNotGiveChildBonusToNonSingleFather()
    {
        var employee = CreateEmployee(
            new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero),
            Gender.Male,
            childrenUnder14: 3);

        var breakdown = VacationEntitlementCalculator.Calculate(
            employee,
            new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero));

        Assert.Equal(2, breakdown.ExperienceBonusDays);
        Assert.Equal(0, breakdown.ChildrenUnder14Days);
        Assert.Equal(2, breakdown.TotalAdditionalDays);
    }

    [Fact]
    public void Calculate_GivesChildBonusToSingleFather()
    {
        var employee = CreateEmployee(
            new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
            Gender.Male,
            childrenUnder14: 2,
            singleParent: true);

        var breakdown = VacationEntitlementCalculator.Calculate(
            employee,
            new DateTimeOffset(2025, 6, 1, 0, 0, 0, TimeSpan.Zero));

        Assert.Equal(2, breakdown.ChildrenUnder14Days);
        Assert.Equal(2, breakdown.TotalAdditionalDays);
    }

    private static Employee CreateEmployee(
        DateTimeOffset startWorkDate,
        Gender gender,
        int childrenUnder14 = 0,
        bool karabakhWorker = false,
        bool disabledChild = false,
        bool singleParent = false) => Employee.Create(
            1,
            "Ali",
            "Mammadov",
            "Rashad",
            gender,
            startWorkDate,
            Guid.NewGuid(),
            Guid.NewGuid(),
            childrenUnder14Count: childrenUnder14,
            isKarabakhWorker: karabakhWorker,
            hasDisabledChild: disabledChild,
            isSingleParent: singleParent);
}
