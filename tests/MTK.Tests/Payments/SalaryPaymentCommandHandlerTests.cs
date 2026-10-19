using Moq;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.SalaryPayments.Commands.CreateSalaryPayment;
using MTK.Modules.Payments.Domain.Employees;
using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.Payroll;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Domain.SalaryPayments;
using Xunit;

namespace MTK.Tests.Payments;

public sealed class SalaryPaymentCommandHandlerTests
{
    [Fact]
    public async Task CreateHandler_BuildsPaymentFromEmployeeBonusAndHourlyAndFixedAdjustments()
    {
        var employeeRepository = new Mock<IEmployeeRepository>();
        var paymentRepository = new Mock<ISalaryPaymentRepository>();
        var bonusRepository = new Mock<IPayrollBonusRepository>();
        var adjustmentRepository = new Mock<IPayrollAdjustmentRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var employeeId = Guid.NewGuid();
        var otherEmployeeId = Guid.NewGuid();
        var employee = Employee.Create(employeeId, "Aysel Aliyeva", 3200m);
        employeeRepository.Setup(x => x.GetByIdDefaultAsync(employeeId, It.IsAny<CancellationToken>())).ReturnsAsync(employee);
        paymentRepository.Setup(x => x.GetByEmployeeAndPeriodAsync(employeeId, 2026, 8, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SalaryPayment?)null);
        bonusRepository.Setup(x => x.GetForPeriodAsync(2026, 8, It.IsAny<CancellationToken>())).ReturnsAsync(new List<PayrollBonus>
        {
            PayrollBonus.Create(Guid.NewGuid(), employeeId, 200m, 8, 2026),
            PayrollBonus.Create(Guid.NewGuid(), otherEmployeeId, 900m, 8, 2026)
        });
        adjustmentRepository.Setup(x => x.GetForPeriodAsync(2026, 8, It.IsAny<CancellationToken>())).ReturnsAsync(new List<PayrollAdjustment>
        {
            PayrollAdjustment.Create(Guid.NewGuid(), employeeId, PayrollAdjustmentReason.UnpaidLeave, -8m, 160m, 8, 2026),
            PayrollAdjustment.Create(Guid.NewGuid(), employeeId, PayrollAdjustmentReason.VacationCompensation, 0m, 0m, 8, 2026, 350m)
        });
        SalaryPayment? added = null;
        paymentRepository.Setup(x => x.Add(It.IsAny<SalaryPayment>())).Callback<SalaryPayment>(payment => added = payment);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateSalaryPaymentCommandHandler(employeeRepository.Object, paymentRepository.Object,
            bonusRepository.Object, adjustmentRepository.Object, unitOfWork.Object);
        var result = await handler.Handle(new CreateSalaryPaymentCommand(employeeId, 8, 2026,
            PaymentMethod.BankTransfer, new DateTimeOffset(2026, 9, 5, 0, 0, 0, TimeSpan.Zero)), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(added);
        Assert.Equal(3200m, added!.BaseSalary);
        Assert.Equal(200m, added.TotalBonuses);
        Assert.Equal(190m, added.TotalAdjustments);
        Assert.Equal(3590m, added.GrossSalary);
        Assert.Equal(4, added.Lines.Count);
        Assert.Equal(3590m, result.Value.GrossSalary);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateHandler_RejectsDuplicatePeriodPayment()
    {
        var employeeRepository = new Mock<IEmployeeRepository>();
        var paymentRepository = new Mock<ISalaryPaymentRepository>();
        var bonusRepository = new Mock<IPayrollBonusRepository>();
        var adjustmentRepository = new Mock<IPayrollAdjustmentRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var employeeId = Guid.NewGuid();
        employeeRepository.Setup(x => x.GetByIdDefaultAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Employee.Create(employeeId, "Employee", 1000m));
        paymentRepository.Setup(x => x.GetByEmployeeAndPeriodAsync(employeeId, 2026, 8, It.IsAny<CancellationToken>()))
            .ReturnsAsync(SalaryPayment.Create(employeeId, "Employee", 8, 2026, 1000m, DateTimeOffset.UtcNow, PaymentMethod.Cash));

        var handler = new CreateSalaryPaymentCommandHandler(employeeRepository.Object, paymentRepository.Object,
            bonusRepository.Object, adjustmentRepository.Object, unitOfWork.Object);
        var result = await handler.Handle(new CreateSalaryPaymentCommand(employeeId, 8, 2026,
            PaymentMethod.Cash, DateTimeOffset.UtcNow), CancellationToken.None);

        Assert.True(result.IsFailure);
        bonusRepository.Verify(x => x.GetForPeriodAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
