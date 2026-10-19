using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.Charges.Services;
using MTK.Modules.Payments.Application.Payments.Services;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Contracts;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Services;
using Xunit;

namespace MTK.Tests.Payments;

public sealed class CustomerChargeGenerationServiceTests
{
    [Fact]
    public async Task GenerateScheduledCharges_CreatesOnlyBillableRecurringContractServices()
    {
        var contracts = new Mock<IContractRepository>();
        var charges = new Mock<ICustomerChargeRepository>();
        var allocation = new Mock<IPaymentAllocationService>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var contract = CreateActiveCustomerContract();
        contract.AddService("Monthly", 100m, BillingPeriod.Monthly);
        contract.AddService("Quarterly", 300m, BillingPeriod.Quarterly);
        contract.AddService("Yearly", 1200m, BillingPeriod.Yearly);
        contract.AddService("One time", 50m, BillingPeriod.OneTime);
        contracts.Setup(x => x.ListActiveWithServicesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Contract> { contract });
        charges.Setup(x => x.GetBilledServiceIdsForPeriodAsync("2026-01", true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<Guid>());
        var transaction = new Mock<ITransaction>();
        charges.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(transaction.Object);
        allocation.Setup(x => x.ApplyAdvanceToChargesAsync(It.IsAny<IReadOnlyCollection<Charge>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());
        var created = new List<Charge>();
        charges.Setup(x => x.Add(It.IsAny<Charge>())).Callback<Charge>(created.Add);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = new CustomerChargeGenerationService(contracts.Object, charges.Object, allocation.Object,
            unitOfWork.Object, NullLogger<CustomerChargeGenerationService>.Instance);
        var count = await service.GenerateScheduledChargesAsync("2026-01");

        Assert.Equal(3, count);
        Assert.Equal(3, created.Count);
        Assert.Equal(new[] { 100m, 300m, 1200m }, created.Select(c => c.Amount).OrderBy(x => x));
        Assert.All(created, charge => Assert.Equal("2026-01", charge.Period));
        allocation.Verify(x => x.ApplyAdvanceToChargesAsync(It.Is<IReadOnlyCollection<Charge>>(items => items.Count == 3), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        transaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GenerateScheduledCharges_SkipsServicesAlreadyBilledForPeriod()
    {
        var contracts = new Mock<IContractRepository>();
        var charges = new Mock<ICustomerChargeRepository>();
        var allocation = new Mock<IPaymentAllocationService>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var contract = CreateActiveCustomerContract();
        var monthly = contract.AddService("Monthly", 100m, BillingPeriod.Monthly);
        contracts.Setup(x => x.ListActiveWithServicesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Contract> { contract });
        charges.Setup(x => x.GetBilledServiceIdsForPeriodAsync("2026-01", true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<Guid> { monthly.Id });

        var service = new CustomerChargeGenerationService(contracts.Object, charges.Object, allocation.Object,
            unitOfWork.Object, NullLogger<CustomerChargeGenerationService>.Instance);
        var count = await service.GenerateScheduledChargesAsync("2026-01");

        Assert.Equal(0, count);
        charges.Verify(x => x.Add(It.IsAny<Charge>()), Times.Never);
        charges.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        allocation.Verify(x => x.ApplyAdvanceToChargesAsync(It.IsAny<IReadOnlyCollection<Charge>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Contract CreateActiveCustomerContract()
    {
        var contract = Contract.CreateForCustomer("CUSTOMER-1", Guid.NewGuid(),
            new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 12, 31, 0, 0, 0, TimeSpan.Zero));
        return contract;
    }
}
