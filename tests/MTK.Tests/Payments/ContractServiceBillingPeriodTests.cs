using MTK.Modules.Payments.Domain.Contracts;
using Xunit;

namespace MTK.Tests.Payments;

public sealed class ContractServiceBillingPeriodTests
{
    private static readonly DateTimeOffset ContractStart = new(2026, 2, 1, 0, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset ContractEnd = new(2027, 1, 31, 0, 0, 0, TimeSpan.Zero);

    [Fact]
    public void MonthlyService_IsBillableForEveryMonthIncludingEndMonth()
    {
        var service = CreateService(BillingPeriod.Monthly);
        var periods = Months("2026-02", 12);

        Assert.Equal(12, periods.Count(period => service.IsBillableFor(ContractStart, ContractEnd, period)));
        Assert.True(service.IsBillableFor(ContractStart, ContractEnd, "2027-01"));
        Assert.False(service.IsBillableFor(ContractStart, ContractEnd, "2027-02"));
        Assert.False(service.IsBillableFor(ContractStart, ContractEnd, "2026-01"));
    }

    [Fact]
    public void QuarterlyService_IsBillableEveryThreeMonthsFromServiceStart()
    {
        var service = CreateService(BillingPeriod.Quarterly);
        var periods = Months("2026-02", 12);

        Assert.Equal(new[] { "2026-02", "2026-05", "2026-08", "2026-11" },
            periods.Where(period => service.IsBillableFor(ContractStart, ContractEnd, period)));
    }

    [Fact]
    public void YearlyService_IsBillableOnItsStartMonthAndNotAgainBeforeContractEnds()
    {
        var service = CreateService(BillingPeriod.Yearly);
        var periods = Months("2026-02", 12);

        Assert.Equal(new[] { "2026-02" },
            periods.Where(period => service.IsBillableFor(ContractStart, ContractEnd, period)));
        Assert.False(service.IsBillableFor(ContractStart, ContractEnd, "2027-02"));
    }

    [Fact]
    public void ServiceSpecificDatesOverrideContractDatesAndAreInclusiveByMonth()
    {
        var service = CreateService(BillingPeriod.Monthly,
            new DateTimeOffset(2026, 4, 25, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 6, 2, 0, 0, 0, TimeSpan.Zero));

        Assert.False(service.IsBillableFor(ContractStart, ContractEnd, "2026-03"));
        Assert.True(service.IsBillableFor(ContractStart, ContractEnd, "2026-04"));
        Assert.True(service.IsBillableFor(ContractStart, ContractEnd, "2026-06"));
        Assert.False(service.IsBillableFor(ContractStart, ContractEnd, "2026-07"));
    }

    [Fact]
    public void OneTimeInactiveAndMalformedPeriodServicesAreNotBillable()
    {
        var oneTime = CreateService(BillingPeriod.OneTime);
        var monthly = CreateService(BillingPeriod.Monthly);
        var contract = Contract.Create("TEST-2", Guid.NewGuid(), ContractStart, ContractEnd);
        var inactive = contract.AddService("Inactive", 50m, BillingPeriod.Monthly);
        contract.DeactivateService(inactive.Id);

        Assert.False(oneTime.IsBillableFor(ContractStart, ContractEnd, "2026-02"));
        Assert.False(inactive.IsBillableFor(ContractStart, ContractEnd, "2026-02"));
        Assert.False(monthly.IsBillableFor(ContractStart, ContractEnd, "2026-13"));
        Assert.False(monthly.IsBillableFor(ContractStart, ContractEnd, "not-a-period"));
    }

    private static ContractService CreateService(
        BillingPeriod period,
        DateTimeOffset? serviceStart = null,
        DateTimeOffset? serviceEnd = null)
    {
        var contract = Contract.Create("TEST-1", Guid.NewGuid(), ContractStart, ContractEnd);
        return contract.AddService("Service", 100m, period, serviceStartDate: serviceStart, serviceEndDate: serviceEnd);
    }

    private static List<string> Months(string firstPeriod, int count)
    {
        var first = DateTime.ParseExact(firstPeriod, "yyyy-MM", System.Globalization.CultureInfo.InvariantCulture);
        return Enumerable.Range(0, count)
            .Select(offset => first.AddMonths(offset).ToString("yyyy-MM", System.Globalization.CultureInfo.InvariantCulture))
            .ToList();
    }
}
