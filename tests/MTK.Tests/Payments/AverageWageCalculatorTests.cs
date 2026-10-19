using Moq;
using MTK.Modules.Payments.Application.Services.AverageWage;
using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Domain.SalaryPayments;
using Xunit;

namespace MTK.Tests.Payments;

public sealed class AverageWageCalculatorTests
{
    [Fact]
    public async Task CalculateAsync_CombinesSourcesAndPrefersPayslipForSameMonth()
    {
        var salaryRepository = new Mock<ISalaryPaymentRepository>();
        var payrollRepository = new Mock<IPayrollRunRepository>();
        var employeeId = Guid.NewGuid();
        var salaryPayment = SalaryPayment.Create(employeeId, "Employee", 1, 2024, 1000m,
            DateTimeOffset.UtcNow, PaymentMethod.BankTransfer);
        salaryPayment.AddLine(SalaryPaymentLineType.Bonus, "Bonus", 100m);
        salaryRepository.Setup(x => x.GetByEmployeeAndPeriodRangeAsync(employeeId, 2024, 1, 2024, 12, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SalaryPayment> { salaryPayment });
        payrollRepository.Setup(x => x.GetPayslipsByEmployeeAndPeriodRangeAsync(employeeId, 2024, 1, 2024, 12, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PayslipWithPeriodDto>
            {
                new(2024, 1, 1400m, 0m, 0m, 1400m),
                new(2024, 2, 1200m, 0m, 0m, 1200m)
            });

        var calculator = new AverageWageCalculator(salaryRepository.Object, payrollRepository.Object);
        var result = await calculator.CalculateAsync(employeeId, new DateTime(2025, 1, 15));

        Assert.Equal(2, result.MonthsIncluded);
        Assert.Equal(1300m, result.AverageMonthlyWage);
        Assert.Equal(42.76m, result.AverageDailyWage);
        Assert.Equal(1400m, result.Breakdown[0].GrossWage);
        Assert.Equal(2024, result.FromDate.Year);
        Assert.Equal(1, result.FromDate.Month);
        Assert.Equal(12, result.ToDate.Month);
        salaryRepository.Verify(x => x.GetByEmployeeAndPeriodRangeAsync(employeeId, 2024, 1, 2024, 12, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CalculateAsync_ThrowsWhenNoWageHistoryExists()
    {
        var salaryRepository = new Mock<ISalaryPaymentRepository>();
        var payrollRepository = new Mock<IPayrollRunRepository>();
        salaryRepository.Setup(x => x.GetByEmployeeAndPeriodRangeAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SalaryPayment>());
        payrollRepository.Setup(x => x.GetPayslipsByEmployeeAndPeriodRangeAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PayslipWithPeriodDto>());

        var calculator = new AverageWageCalculator(salaryRepository.Object, payrollRepository.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => calculator.CalculateAsync(Guid.NewGuid(), new DateTime(2025, 1, 15)));
    }
}
