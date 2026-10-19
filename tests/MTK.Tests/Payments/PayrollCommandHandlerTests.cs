using Moq;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.CompanyBalances.Services;
using MTK.Modules.Payments.Application.Payroll;
using MTK.Modules.Payments.Application.Payroll.Commands.ApprovePayrollRun;
using MTK.Modules.Payments.Application.Payroll.Commands.PayPayrollRun;
using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.Payroll;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Domain.Transactions;
using Xunit;

namespace MTK.Tests.Payments;

public sealed class PayrollCommandHandlerTests
{
    [Fact]
    public async Task ApproveHandler_ApprovesCalculatedRunAndSaves()
    {
        var repository = new Mock<IPayrollRunRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var run = CreateCalculatedRun();
        repository.Setup(x => x.GetWithPayslipsAsync(run.Id, It.IsAny<CancellationToken>())).ReturnsAsync(run);

        var handler = new ApprovePayrollRunCommandHandler(repository.Object, unitOfWork.Object);
        var result = await handler.Handle(new ApprovePayrollRunCommand(run.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(PayrollRunStatus.Approved, run.Status);
        Assert.NotNull(result.Value.ApprovedAt);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task PayHandler_PaysRunCreatesTransactionPerPayslipAndRecalculatesBalance()
    {
        var payrollRepository = new Mock<IPayrollRunRepository>();
        var transactions = new Mock<ITransactionRepository>();
        var companyBalance = new Mock<ICompanyBalanceService>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var run = PayrollRun.Create(2026, 8);
        run.AddPayslip(Guid.NewGuid(), "Aysel Aliyeva", PayrollCalculator.Calculate(3000m, 200m, -100m));
        run.AddPayslip(Guid.NewGuid(), "Murad Hasanov", PayrollCalculator.Calculate(2500m, 0m, 0m));
        run.MarkCalculated();
        run.Approve();
        payrollRepository.Setup(x => x.GetWithPayslipsAsync(run.Id, It.IsAny<CancellationToken>())).ReturnsAsync(run);

        var handler = new PayPayrollRunCommandHandler(payrollRepository.Object, transactions.Object,
            companyBalance.Object, unitOfWork.Object);
        var result = await handler.Handle(new PayPayrollRunCommand(run.Id, PaymentMethod.BankTransfer), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(PayrollRunStatus.Paid, run.Status);
        Assert.All(run.Payslips, payslip => Assert.NotNull(payslip.PaidAt));
        Assert.Equal(2, result.Value.Payslips.Count);
        transactions.Verify(x => x.Add(It.IsAny<Transaction>()), Times.Exactly(2));
        companyBalance.Verify(x => x.RecalculateAsync(It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task PayHandler_RejectsRunThatHasNotBeenApproved()
    {
        var payrollRepository = new Mock<IPayrollRunRepository>();
        var transactions = new Mock<ITransactionRepository>();
        var companyBalance = new Mock<ICompanyBalanceService>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var run = CreateCalculatedRun();
        payrollRepository.Setup(x => x.GetWithPayslipsAsync(run.Id, It.IsAny<CancellationToken>())).ReturnsAsync(run);

        var handler = new PayPayrollRunCommandHandler(payrollRepository.Object, transactions.Object,
            companyBalance.Object, unitOfWork.Object);
        var result = await handler.Handle(new PayPayrollRunCommand(run.Id), CancellationToken.None);

        Assert.True(result.IsFailure);
        transactions.Verify(x => x.Add(It.IsAny<Transaction>()), Times.Never);
        companyBalance.Verify(x => x.RecalculateAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private static PayrollRun CreateCalculatedRun()
    {
        var run = PayrollRun.Create(2026, 8);
        run.AddPayslip(Guid.NewGuid(), "Aysel Aliyeva", PayrollCalculator.Calculate(3000m, 0m, 0m));
        run.MarkCalculated();
        return run;
    }
}
