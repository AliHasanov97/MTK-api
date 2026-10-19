using MTK.Modules.Payments.Domain.Transactions;
using Xunit;

namespace MTK.Tests.Transactions;

public sealed class TransactionTests
{
    [Fact]
    public void Create_WithValidValues_PreservesTransactionData()
    {
        var date = new DateTimeOffset(2026, 10, 19, 12, 30, 0, TimeSpan.Zero);

        var transaction = Transaction.Create(
            TransactionDirection.Income,
            "Resident payment",
            125.50m,
            "October payment",
            date);

        Assert.Equal(TransactionDirection.Income, transaction.Direction);
        Assert.Equal("Resident payment", transaction.Category);
        Assert.Equal(125.50m, transaction.Amount);
        Assert.Equal("October payment", transaction.Description);
        Assert.Equal(date, transaction.TransactionDate);
        Assert.Equal("LedgerEntry", transaction.DocumentType);
        Assert.Equal(transaction.Id, transaction.ReferenceId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithNonPositiveAmount_ThrowsArgumentException(decimal amount)
    {
        var exception = Assert.Throws<ArgumentException>(() => Transaction.Create(
            TransactionDirection.Expense,
            "Purchase",
            amount,
            null,
            DateTimeOffset.UtcNow));

        Assert.Contains("must be positive", exception.Message);
    }

    [Fact]
    public void ForPurchase_SetsExpenseAndPurchaseReference()
    {
        var purchaseId = Guid.NewGuid();
        var date = new DateTimeOffset(2026, 10, 19, 12, 30, 0, TimeSpan.Zero);

        var transaction = Transaction.ForPurchase(
            purchaseId,
            80m,
            date,
            "Supplier",
            "INV-42",
            "Office supplies");

        Assert.Equal(TransactionDirection.Expense, transaction.Direction);
        Assert.Equal(80m, transaction.Amount);
        Assert.Equal(purchaseId, transaction.SourcePurchaseId);
        Assert.Equal("Purchase", transaction.DocumentType);
        Assert.Equal(purchaseId, transaction.ReferenceId);
        Assert.Contains("Supplier", transaction.Description);
        Assert.Contains("INV-42", transaction.Description);
        Assert.Contains("Office supplies", transaction.Description);
    }
}
