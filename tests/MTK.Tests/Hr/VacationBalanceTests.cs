using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.VacationBalances;
using Xunit;

namespace MTK.Tests.Hr;

public sealed class VacationBalanceTests
{
    [Fact]
    public void Create_ComputesWorkYearAndTotalEntitlement()
    {
        var balance = VacationBalance.Create(
            Guid.NewGuid(),
            new DateOnly(2026, 4, 1),
            21,
            new AdditionalVacationDaysBreakdown
            {
                ExperienceBonusDays = 2,
                KarabakhWorkerDays = 5,
                ChildrenUnder14Days = 2,
                DisabledChildDays = 5
            });

        Assert.Equal(new DateOnly(2026, 4, 1), balance.WorkYearStart);
        Assert.Equal(new DateOnly(2027, 3, 31), balance.WorkYearEnd);
        Assert.Equal(14, balance.AdditionalDays);
        Assert.Equal(35, balance.TotalEntitledDays);
        Assert.Equal(35, balance.RemainingDays);
    }

    [Fact]
    public void RegisterUsedAndCompensatedDays_ReduceRemainingBalance()
    {
        var balance = CreateBalance(30);

        balance.RegisterUsedDays(7);
        balance.RegisterCompensatedDays(3);

        Assert.Equal(7, balance.UsedDays);
        Assert.Equal(3, balance.CompensatedDays);
        Assert.Equal(20, balance.RemainingDays);
    }

    [Fact]
    public void RegisterUsedDays_WhenRequestExceedsRemaining_Throws()
    {
        var balance = CreateBalance(5);

        Assert.Throws<InvalidOperationException>(() => balance.RegisterUsedDays(6));
        Assert.Equal(5, balance.RemainingDays);
    }

    [Fact]
    public void ReturnUnusedDays_DoesNotMakeUsedDaysNegative()
    {
        var balance = CreateBalance(20);
        balance.RegisterUsedDays(4);

        balance.ReturnUnusedDays(10);

        Assert.Equal(0, balance.UsedDays);
        Assert.Equal(20, balance.RemainingDays);
    }

    private static VacationBalance CreateBalance(int days) => VacationBalance.Create(
        Guid.NewGuid(),
        new DateOnly(2026, 1, 1),
        days,
        new AdditionalVacationDaysBreakdown());
}
