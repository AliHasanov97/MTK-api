using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Parties;
using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Domain.Transactions;

/// <summary>
/// A general financial ledger entry. Completed payments post here through the
/// domain event: an owner payment is money received (Income), a vendor payment is
/// money paid out (Expense). Kept deliberately simple (no allocation/FIFO logic,
/// no structured owner/property/period columns) since the debt settlement happens
/// on the Charge/Payment side — "who paid, for which unit, for which month" lives
/// as plain text in Description, assembled once by the event handler.
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
    ///
    /// <paramref name="vendorName"/>, <paramref name="propertyLabel"/> and
    /// <paramref name="periodLabel"/> are assembled by the event handler — the
    /// vendor, the property's number and the payment's own allocations all live
    /// in this module, unlike the resident/owner's name, which doesn't.
    ///
    /// <paramref name="amount"/> defaults to the payment's own total, but a caller
    /// settling several charges in one payment (a vendor's bulk/FIFO payment) can
    /// pass one charge's own allocated slice instead — one ledger line per charge,
    /// each with its own clean description, instead of one line for the whole
    /// payment with every charge's description mashed together.
    /// </summary>
    public static Transaction ForPayment(
        Payment payment,
        string? vendorName = null,
        string? propertyLabel = null,
        string? periodLabel = null,
        decimal? amount = null) =>
        payment.PartyType == PartyType.Vendor
            ? Create(
                TransactionDirection.Expense,
                // Admin faydalı kateqoriya kimi tədarükçünün adını görmək istəyir,
                // sahə boş qalarsa (vendor tapılmayıbsa) generic ada geri qayıdır.
                !string.IsNullOrWhiteSpace(vendorName) ? vendorName : VendorPaymentCategory,
                amount ?? payment.Amount,
                Describe(payment, propertyLabel, periodLabel),
                payment.PaymentDate.ToUniversalTime())
            : Create(
                TransactionDirection.Income,
                ResidentPaymentCategory,
                amount ?? payment.Amount,
                Describe(payment, propertyLabel, periodLabel),
                payment.PaymentDate.ToUniversalTime());

    // No Delete(): the ledger is append-only.

    // A payment entry opens with a plain-language sentence naming who/what it's
    // for and which period it covers ("290 nömrəli mənzil Sentyabr 2026 ayı üçün
    // ödəniş"), then the method and the admin's own note — so the ledger reads
    // on its own instead of being joined back to the payment to find out. The
    // vendor's own name isn't repeated here — it's already the Category for a
    // vendor payment.
    private static string Describe(Payment payment, string? propertyLabel, string? periodLabel)
    {
        var parts = new List<string>();

        var lead = BuildLeadSentence(payment, propertyLabel, periodLabel);
        if (!string.IsNullOrWhiteSpace(lead))
            parts.Add(lead);

        parts.Add(MethodName(payment.PaymentMethod));
        if (!string.IsNullOrWhiteSpace(payment.Notes))
            parts.Add(payment.Notes);
        return string.Join(" · ", parts);
    }

    // propertyLabel arrives as "{number} nömrəli mənzil"/"{number} nömrəli qaraj";
    // periodLabel as one or more "{Ay} {il}" entries joined with ", ".
    private static string? BuildLeadSentence(Payment payment, string? propertyLabel, string? periodLabel)
    {
        if (string.IsNullOrWhiteSpace(propertyLabel))
        {
            // A general (untargeted) resident payment with no property to name —
            // typically the whole amount landed as an unapplied advance (no open
            // debt to allocate against). Without this, the description used to
            // just lead with the bare payment method ("Nağd"), reading like a
            // dangling word instead of a sentence.
            return payment.PartyType == PartyType.Owner
                ? "Sakin öz hesabına mədaxil etmişdir"
                : null;
        }

        if (string.IsNullOrWhiteSpace(periodLabel))
            return $"{propertyLabel} üçün ödəniş";

        var monthWord = periodLabel.Contains(',') ? "ayları" : "ayı";
        return $"{propertyLabel} {periodLabel} {monthWord} üçün ödəniş";
    }

    private static string MethodName(PaymentMethod method) => method switch
    {
        PaymentMethod.Cash => "Nağd",
        PaymentMethod.BankTransfer => "Bank köçürməsi",
        PaymentMethod.Card => "Kart",
        _ => method.ToString()
    };
}
