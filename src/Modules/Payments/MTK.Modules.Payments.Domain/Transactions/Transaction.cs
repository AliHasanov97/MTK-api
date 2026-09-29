using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Payments;

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
    public const string PaymentReversalCategory = "Ödəniş ləğvi";

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
    /// Money handed back when a payment is cancelled. The original entry stays put:
    /// a cancellation is corrected by an opposing entry, not by rewriting history.
    /// </summary>
    public static Transaction ForPaymentReversal(Payment payment, DateTimeOffset occurredOnUtc) =>
        Create(
            TransactionDirection.Expense,
            PaymentReversalCategory,
            payment.Amount,
            Describe(payment),
            occurredOnUtc);

    // No Delete(): the ledger is append-only. A wrong entry is corrected by the
    // opposing entry its payment's reversal posts, never by removing history.

    // A payment entry names the method (and reference, when there is one) so the
    // ledger reads on its own instead of being joined back to the payment.
    private static string Describe(Payment payment)
    {
        var method = payment.PaymentMethod switch
        {
            PaymentMethod.Cash => "Nağd",
            PaymentMethod.BankTransfer => "Bank köçürməsi",
            PaymentMethod.Card => "Kart",
            _ => payment.PaymentMethod.ToString()
        };

        return payment.Reference is null ? method : $"{method} · {payment.Reference}";
    }
}
