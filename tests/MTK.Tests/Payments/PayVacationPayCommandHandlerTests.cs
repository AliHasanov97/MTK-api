using Moq;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.VacationPays.Commands.PayVacationPay;
using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Domain.VacationPays;
using Xunit;

namespace MTK.Tests.Payments;

public sealed class PayVacationPayCommandHandlerTests
{
    [Fact]
    public async Task Handle_MarksPayAsPaidAndSaves()
    {
        var repository = new Mock<IVacationPayRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var payId = Guid.NewGuid();
        var pay = VacationPay.Create(Guid.NewGuid(), Guid.NewGuid(), "Employee", 1500m, 49.34m, 10,
            493.40m, new DateOnly(2026, 8, 10), new DateOnly(2026, 8, 19), new DateOnly(2026, 8, 7));
        repository.Setup(x => x.GetByIdDefaultAsync(payId, It.IsAny<CancellationToken>())).ReturnsAsync(pay);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new PayVacationPayCommandHandler(repository.Object, unitOfWork.Object);
        var result = await handler.Handle(new PayVacationPayCommand(payId, PaymentMethod.Card), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(VacationPayStatus.Paid, pay.Status);
        Assert.Equal(PaymentMethod.Card, pay.PaymentMethod);
        Assert.NotNull(pay.PaidAt);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenPayDoesNotExist_ReturnsFailureWithoutSaving()
    {
        var repository = new Mock<IVacationPayRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var payId = Guid.NewGuid();
        repository.Setup(x => x.GetByIdDefaultAsync(payId, It.IsAny<CancellationToken>())).ReturnsAsync((VacationPay?)null);

        var handler = new PayVacationPayCommandHandler(repository.Object, unitOfWork.Object);
        var result = await handler.Handle(new PayVacationPayCommand(payId, PaymentMethod.Cash), CancellationToken.None);

        Assert.True(result.IsFailure);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
