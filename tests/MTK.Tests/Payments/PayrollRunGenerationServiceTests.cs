using Microsoft.Extensions.Options;
using Moq;
using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.Payroll.Services;
using MTK.Modules.Payments.Domain.Employees;
using MTK.Modules.Payments.Domain.Payroll;
using MTK.Modules.Payments.Domain.Repositories;
using Xunit;

namespace MTK.Tests.Payments;

public sealed class PayrollRunGenerationServiceTests
{
    [Fact]
    public async Task GenerateAsync_UsesFixedAdjustmentsAndIgnoresInactiveEmployees()
    {
        var runRepository = new Mock<IPayrollRunRepository>();
        var employeeRepository = new Mock<IEmployeeRepository>();
        var bonusRepository = new Mock<IPayrollBonusRepository>();
        var adjustmentRepository = new Mock<IPayrollAdjustmentRepository>();
        var dateTimeProvider = new Mock<IDateTimeProvider>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var employeeId = Guid.NewGuid();
        dateTimeProvider.SetupGet(x => x.UtcNow).Returns(new DateTime(2026, 10, 20));
        runRepository.Setup(x => x.GetByPeriodAsync(2026, 9, It.IsAny<CancellationToken>())).ReturnsAsync((PayrollRun?)null);
        employeeRepository.Setup(x => x.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Employee>
        {
            Employee.Create(employeeId, "Aysel Aliyeva", 3000m),
            Employee.Create(Guid.NewGuid(), "Inactive Employee", 5000m, isActive: false)
        });
        bonusRepository.Setup(x => x.GetForPeriodAsync(2026, 9, It.IsAny<CancellationToken>())).ReturnsAsync(new List<PayrollBonus>
        {
            PayrollBonus.Create(Guid.NewGuid(), employeeId, 200m, 9, 2026)
        });
        adjustmentRepository.Setup(x => x.GetForPeriodAsync(2026, 9, It.IsAny<CancellationToken>())).ReturnsAsync(new List<PayrollAdjustment>
        {
            PayrollAdjustment.Create(Guid.NewGuid(), employeeId, PayrollAdjustmentReason.VacationCompensation,
                0m, 0m, 9, 2026, fixedAmount: 350m)
        });
        PayrollRun? addedRun = null;
        runRepository.Setup(x => x.Add(It.IsAny<PayrollRun>())).Callback<PayrollRun>(run => addedRun = run);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = new PayrollRunGenerationService(runRepository.Object, employeeRepository.Object,
            bonusRepository.Object, adjustmentRepository.Object, dateTimeProvider.Object,
            Options.Create(new PayrollPeriodOptions { TimeZoneId = "UTC" }), unitOfWork.Object);
        var result = await service.GenerateAsync(2026, 9);

        Assert.True(result.IsSuccess);
        Assert.NotNull(addedRun);
        Assert.Equal(PayrollRunStatus.Calculated, addedRun!.Status);
        var payslip = Assert.Single(addedRun.Payslips);
        Assert.Equal(employeeId, payslip.EmployeeId);
        Assert.Equal(3000m, payslip.BaseSalary);
        Assert.Equal(200m, payslip.BonusAmount);
        Assert.Equal(350m, payslip.AdjustmentAmount);
        Assert.Equal(3550m, payslip.GrossSalary);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GenerateAsync_RejectsCurrentMonth()
    {
        var dateTimeProvider = new Mock<IDateTimeProvider>();
        dateTimeProvider.SetupGet(x => x.UtcNow).Returns(new DateTime(2026, 10, 20));
        var runRepository = new Mock<IPayrollRunRepository>();
        var employeeRepository = new Mock<IEmployeeRepository>();
        var bonusRepository = new Mock<IPayrollBonusRepository>();
        var adjustmentRepository = new Mock<IPayrollAdjustmentRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var service = new PayrollRunGenerationService(runRepository.Object, employeeRepository.Object,
            bonusRepository.Object, adjustmentRepository.Object, dateTimeProvider.Object,
            Options.Create(new PayrollPeriodOptions { TimeZoneId = "UTC" }), unitOfWork.Object);

        var result = await service.GenerateAsync(2026, 10);

        Assert.True(result.IsFailure);
        runRepository.Verify(x => x.GetByPeriodAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
