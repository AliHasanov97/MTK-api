using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.VendorPayments;

namespace MTK.Modules.Payments.Domain.Transactions;

/// <summary>
/// A general financial ledger entry that is NOT tied to a resident's Charge/Payment
/// cycle — e.g. an inventory purchase, a maintenance expense, rental income from a
/// shared space. Kept deliberately simple (no allocation/FIFO logic like Payment has)
/// since it isn't settling anyone's debt; it just records money moving in or out.
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
    /// Money received from a resident — the entry a completed payment posts.
    /// </summary>
    public static Transaction ForPayment(Payment payment) =>
        Create(
            TransactionDirection.Income,
            ResidentPaymentCategory,
            payment.Amount,
            Describe(payment),
            payment.PaymentDate.ToUniversalTime());

    /// <summary>
    /// Money paid out to a vendor — the entry a completed vendor payment posts. The
    /// opposite direction of <see cref="ForPayment"/>: here the money leaves us.
    /// </summary>
    public static Transaction ForVendorPayment(VendorPayment payment) =>
        Create(
            TransactionDirection.Expense,
            VendorPaymentCategory,
            payment.Amount,
            DescribeVendorPayment(payment),
            payment.PaymentDate.ToUniversalTime());

    // No Delete(): the ledger is append-only.

    // A payment entry names the method (and reference, when there is one) so the
    // ledger reads on its own instead of being joined back to the payment.
    private static string Describe(Payment payment)
    {
        var method = MethodName(payment.PaymentMethod);
        return payment.Reference is null ? method : $"{method} · {payment.Reference}";
    }

    private static string DescribeVendorPayment(VendorPayment payment)
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
