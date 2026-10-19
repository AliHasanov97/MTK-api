using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.Payments.Services;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Contracts;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Services;
using Xunit;

namespace MTK.Tests.Payments;

public sealed class VendorChargeGenerationServiceTests
{
    [Fact]
    public async Task GenerateScheduledCharges_CreatesMonthlyAndQuarterlyChargesForApplicableMonth()
    {
        var contracts = new Mock<IContractRepository>();
        var charges = new Mock<IVendorChargeRepository>();
        var allocation = new Mock<IPaymentAllocationService>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var contract = Contract.Create("VENDOR-1", Guid.NewGuid(),
            new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 12, 31, 0, 0, 0, TimeSpan.Zero));
        contract.AddService("Monthly", 100m, BillingPeriod.Monthly);
        contract.AddService("Quarterly", 300m, BillingPeriod.Quarterly);
        contract.AddService("Yearly", 1200m, BillingPeriod.Yearly);
        contracts.Setup(x => x.ListActiveWithServicesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Contract> { contract });
        charges.Setup(x => x.GetBilledServiceIdsForPeriodAsync("2026-04", true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<Guid>());
        var transaction = new Mock<ITransaction>();
        charges.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(transaction.Object);
        allocation.Setup(x => x.ApplyAdvanceToChargesAsync(It.IsAny<IReadOnlyCollection<Charge>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());
        var created = new List<Charge>();
        charges.Setup(x => x.Add(It.IsAny<Charge>())).Callback<Charge>(created.Add);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = new VendorChargeGenerationService(contracts.Object, charges.Object, allocation.Object,
            unitOfWork.Object, NullLogger<VendorChargeGenerationService>.Instance);
        var count = await service.GenerateScheduledChargesAsync("2026-04");

        Assert.Equal(2, count);
        Assert.Equal(new[] { 100m, 300m }, created.Select(c => c.Amount).OrderBy(amount => amount));
        Assert.All(created, charge => Assert.Equal("2026-04", charge.Period));
        transaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
