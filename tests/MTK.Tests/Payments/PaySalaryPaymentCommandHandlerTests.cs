using Moq;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.SalaryPayments.Commands.PaySalaryPayment;
using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Domain.SalaryPayments;
using Xunit;

namespace MTK.Tests.Payments;

public sealed class PaySalaryPaymentCommandHandlerTests
{
    [Fact]
    public async Task Handle_PaysDraftPaymentAndSaves()
    {
        var repository = new Mock<ISalaryPaymentRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var id = Guid.NewGuid();
        var payment = SalaryPayment.Create(Guid.NewGuid(), "Employee", 8, 2026, 2000m,
            DateTimeOffset.UtcNow, PaymentMethod.BankTransfer);
        payment.AddLine(SalaryPaymentLineType.BaseSalary, "Base salary", 2000m);
        repository.Setup(x => x.GetByIdWithLinesAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(payment);

        var handler = new PaySalaryPaymentCommandHandler(repository.Object, unitOfWork.Object);
        var result = await handler.Handle(new PaySalaryPaymentCommand(id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(SalaryPaymentStatus.Paid, payment.Status);
        Assert.Equal(SalaryPaymentStatus.Paid, result.Value.Status);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenPaymentMissing_ReturnsFailureWithoutSaving()
    {
        var repository = new Mock<ISalaryPaymentRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var id = Guid.NewGuid();
        repository.Setup(x => x.GetByIdWithLinesAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((SalaryPayment?)null);

        var handler = new PaySalaryPaymentCommandHandler(repository.Object, unitOfWork.Object);
        var result = await handler.Handle(new PaySalaryPaymentCommand(id), CancellationToken.None);

        Assert.True(result.IsFailure);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
