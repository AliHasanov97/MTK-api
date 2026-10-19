using MTK.Modules.Warehouse.Domain.WarehouseStock;
using Xunit;

namespace MTK.Tests.Warehouse;

public sealed class WarehouseStockTests
{
    [Fact]
    public void IncreaseAndDecreaseStock_UpdateBalance()
    {
        var stock = WarehouseStock.Create(Guid.NewGuid(), 10m);
        var transactionDate = new DateTimeOffset(2026, 10, 19, 12, 0, 0, TimeSpan.Zero);

        stock.IncreaseStock(5m, transactionDate);
        stock.DecreaseStock(3m, transactionDate);

        Assert.Equal(12m, stock.QuantityOnHand);
        Assert.Equal(transactionDate, stock.LastTransactionDate);
        Assert.True(stock.HasSufficientStock(12m));
        Assert.False(stock.HasSufficientStock(13m));
    }

    [Fact]
    public void DecreaseStock_WhenInsufficient_ThrowsWithoutChangingBalance()
    {
        var stock = WarehouseStock.Create(Guid.NewGuid(), 2m);

        Assert.Throws<InvalidOperationException>(() => stock.DecreaseStock(3m));

        Assert.Equal(2m, stock.QuantityOnHand);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void IncreaseStock_WithNonPositiveQuantity_Throws(decimal quantity)
    {
        var stock = WarehouseStock.Create(Guid.NewGuid());

        Assert.Throws<ArgumentException>(() => stock.IncreaseStock(quantity));
    }
}
