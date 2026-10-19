using MTK.Modules.Payments.Domain.Payroll;
using Xunit;

namespace MTK.Tests.Payments;

public sealed class PayrollCalculatorTests
{
    [Fact]
    public void Calculate_AddsBonusAndSignedAdjustmentAndUsesGrossAsNet()
    {
        var result = PayrollCalculator.Calculate(2000m, 250m, -100m);

        Assert.Equal(2000m, result.BaseSalary);
        Assert.Equal(250m, result.BonusAmount);
        Assert.Equal(-100m, result.AdjustmentAmount);
        Assert.Equal(2150m, result.GrossSalary);
        Assert.Equal(2150m, result.FinalNetSalary);
        Assert.Equal(2150m, result.TotalEmployerCost);
    }

    [Fact]
    public void Calculate_DoesNotAllowNegativeGrossSalary()
    {
        var result = PayrollCalculator.Calculate(100m, 0m, -500m);

        Assert.Equal(0m, result.GrossSalary);
        Assert.Equal(0m, result.FinalNetSalary);
    }

    [Theory]
    [InlineData(-1, 0, 0)]
    [InlineData(0, -1, 0)]
    public void Calculate_RejectsNegativeBaseSalaryOrBonus(decimal baseSalary, decimal bonus, decimal adjustment)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => PayrollCalculator.Calculate(baseSalary, bonus, adjustment));
    }
}
