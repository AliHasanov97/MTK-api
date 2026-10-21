using MTK.Modules.Payments.Domain.Parties;
using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Application.Abstractions.Services.Export;

public interface IPaymentReceiptExportService
{
    MemoryStream ExportToPdf(PaymentReceiptData data);
}

/// <summary>One applied charge or expense line, including its property and billing period
/// where available, so the receipt can show exactly which balance received the payment.</summary>
public sealed record PaymentReceiptLine(string Description, decimal Amount, string? Period = null, string? PropertyLabel = null);

/// <summary>
/// Everything the PDF needs, already resolved server-side from Payments' own data (Owner,
/// charge allocations and AuditLog) — the frontend only ever supplies PaymentId (see
/// ExportPaymentReceiptQueryHandler); no other field is caller-supplied.
/// </summary>
public sealed record PaymentReceiptData(
    Guid PaymentId,
    long ReceiptNumber,
    string CompanyName,
    decimal Amount,
    PaymentMethod PaymentMethod,
    DateTimeOffset PaymentDate,
    string? Notes,
    PartyType PartyType,
    string? PayerName,
    // "yyyy-MM" — only set when every charge this payment settled shares one billing
    // period; ambiguous (several months) or absent (pure advance) otherwise.
    string? Period,
    IReadOnlyCollection<PaymentReceiptLine> Lines,
    // The real person/role who recorded this payment (from the Payment entity's own
    // "Created" audit row) — not always a Komendant, could be a Xəzinədar. Null when
    // that audit row's actor can't be resolved (e.g. a legacy payment).
    string? IssuerName,
    string? IssuerTitle,
    string? PayerEmail = null);
