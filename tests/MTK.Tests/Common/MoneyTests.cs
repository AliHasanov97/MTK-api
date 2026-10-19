using MTK.Common.Domain.ValueObjects;
using Xunit;

namespace MTK.Tests.Common;

public sealed class MoneyTests
{
    [Fact]
    public void Add_WithSameCurrency_ReturnsSum()
    {
        var total = new Money(12.25m, Currency.Azn) + new Money(7.75m, Currency.Azn);

        Assert.Equal(20m, total.Amount);
        Assert.Equal(Currency.Azn, total.Currency);
    }

    [Fact]
    public void Add_WithDifferentCurrencies_Throws()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            _ = new Money(10m, Currency.Azn) + new Money(10m, Currency.FromCode("USD")));

        Assert.Contains("Currencies don't match", exception.Message);
    }

    [Fact]
    public void Zero_UsesRequestedCurrency()
    {
        var zero = Money.Zero(Currency.Azn);

        Assert.True(zero.IsZero());
        Assert.Equal(Currency.Azn, zero.Currency);
    }
}
