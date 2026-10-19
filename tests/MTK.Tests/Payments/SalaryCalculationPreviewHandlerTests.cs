using Moq;
using MTK.Modules.Payments.Application.SalaryPayments.Queries.GetSalaryCalculationPreview;
using MTK.Modules.Payments.Domain.Employees;
using MTK.Modules.Payments.Domain.Payroll;
using MTK.Modules.Payments.Domain.Repositories;
using Xunit;

namespace MTK.Tests.Payments;

public sealed class SalaryCalculationPreviewHandlerTests
{
    [Fact]
    public async Task Handle_UsesFixedAndHourlyAdjustmentsAndEmployeeBonuses()
    {
        var employees = new Mock<IEmployeeRepository>();
        var bonuses = new Mock<IPayrollBonusRepository>();
        var adjustments = new Mock<IPayrollAdjustmentRepository>();
        var employeeId = Guid.NewGuid();
        employees.Setup(x => x.GetByIdDefaultAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Employee.Create(employeeId, "Employee", 3200m));
        bonuses.Setup(x => x.GetForPeriodAsync(2026, 8, It.IsAny<CancellationToken>())).ReturnsAsync(new List<PayrollBonus>
        {
            PayrollBonus.Create(Guid.NewGuid(), employeeId, 200m, 8, 2026)
        });
        adjustments.Setup(x => x.GetForPeriodAsync(2026, 8, It.IsAny<CancellationToken>())).ReturnsAsync(new List<PayrollAdjustment>
        {
            PayrollAdjustment.Create(Guid.NewGuid(), employeeId, PayrollAdjustmentReason.UnpaidLeave, -8m, 160m, 8, 2026),
            PayrollAdjustment.Create(Guid.NewGuid(), employeeId, PayrollAdjustmentReason.VacationCompensation, 0m, 0m, 8, 2026, 350m)
        });

        var handler = new GetSalaryCalculationPreviewQueryHandler(employees.Object, bonuses.Object, adjustments.Object);
        var result = await handler.Handle(new GetSalaryCalculationPreviewQuery(employeeId, 8, 2026), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(200m, result.Value.TotalBonuses);
        Assert.Equal(190m, result.Value.TotalAdjustments);
        Assert.Equal(3590m, result.Value.GrossSalary);
        Assert.Equal(new[] { -160m, 350m }, result.Value.Adjustments.Select(a => a.Amount));
    }
}
