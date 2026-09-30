using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Parties;
using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Domain.Transactions;

/// <summary>
/// A general financial ledger entry. Completed payments post here through the
/// domain event: an owner payment is money received (Income), a vendor payment is
/// money paid out (Expense). Kept deliberately simple (no allocation/FIFO logic)
/// since the debt settlement happens on the Charge/Payment side.
/// </summary>
public sealed class Transaction : SearchableEntity
{
    // Ledger categories belong to the domain, not to whoever happens to write the
    // row: the payment handlers and any reporting both key off these.
    public const string ResidentPaymentCategory = "Sakin ödənişi";
    public const string VendorPaymentCategory = "Tədarükçü ödənişi";

    private Transaction() : base() { }

    public TransactionDirection Direction { get; private set; }
    public string Category { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public string? Description { get; private set; }
    public DateTimeOffset TransactionDate { get; private set; }

    public static Transaction Create(
        TransactionDirection direction,
        string category,
        decimal amount,
        string? description,
        DateTimeOffset transactionDate)
    {
        if (amount <= 0)
            throw new ArgumentException("Transaction amount must be positive");

        var transaction = new Transaction
        {
            Direction = direction,
            Category = category,
            Amount = amount,
            Description = description,
            TransactionDate = transactionDate
        };
        transaction.SetCreatedAt();
        return transaction;
    }

    /// <summary>
    /// The entry a completed payment posts. Money received from a resident is an
    /// Income entry; money paid out to a vendor is an Expense entry (opposite
    /// direction), so the unified Payment decides the side.
    /// </summary>
    public static Transaction ForPayment(Payment payment) =>
        payment.PartyType == PartyType.Vendor
            ? Create(
                TransactionDirection.Expense,
                VendorPaymentCategory,
                payment.Amount,
                Describe(payment),
                payment.PaymentDate.ToUniversalTime())
            : Create(
                TransactionDirection.Income,
                ResidentPaymentCategory,
                payment.Amount,
                Describe(payment),
                payment.PaymentDate.ToUniversalTime());

    // No Delete(): the ledger is append-only.

    // A payment entry names the method (and reference, when there is one) so the
    // ledger reads on its own instead of being joined back to the payment.
    private static string Describe(Payment payment)
    {
        var method = MethodName(payment.PaymentMethod);
        return payment.Reference is null ? method : $"{method} · {payment.Reference}";
    }

    private static string MethodName(PaymentMethod method) => method switch
    {
        PaymentMethod.Cash => "Nağd",
        PaymentMethod.BankTransfer => "Bank köçürməsi",
        PaymentMethod.Card => "Kart",
        _ => method.ToString()
    };
}
