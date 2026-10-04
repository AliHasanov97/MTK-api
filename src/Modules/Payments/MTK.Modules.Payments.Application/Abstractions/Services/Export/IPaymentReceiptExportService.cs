using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Parties;
using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Application.Abstractions.Services.Export;

public interface IPaymentReceiptExportService
{
    MemoryStream ExportToPdf(PaymentReceiptData data);
}

/// <summary>One line in the receipt's charge breakdown — which charge this payment covered,
/// for which unit (only set when the payment settled MORE THAN ONE unit — e.g. part
/// cleared an apartment's debt, the rest a garage's) and billing period, and how much of
/// it. A bulk/toplu payment that settles several months and/or several units at once must
/// show every one of these — collapsing them into a single sentence naming only one
/// unit/month would misstate what was actually paid.</summary>
public sealed record PaymentReceiptLine(string Description, decimal Amount, string? Period = null, string? PropertyLabel = null);

/// <summary>
/// Everything the PDF needs, already resolved server-side from Payments' own data (Owner/
/// Building/Apartment shadows, AuditLog) — the frontend only ever supplies PaymentId (see
/// ExportPaymentReceiptQueryHandler); no other field is caller-supplied.
/// </summary>
public sealed record PaymentReceiptData(
    Guid PaymentId,
    decimal Amount,
    PaymentMethod PaymentMethod,
    DateTimeOffset PaymentDate,
    string? Notes,
    PartyType PartyType,
    string? PayerName,
    // Apartment/garage number this payment is for — null for a general (FIFO/advance)
    // payment not tied to one unit.
    string? PropertyNumber,
    PropertyType? PropertyType,
    // "yyyy-MM" — only set when every charge this payment settled shares one billing
    // period; ambiguous (several months) or absent (pure advance) otherwise.
    string? Period,
    IReadOnlyCollection<PaymentReceiptLine> Lines,
    // The real person/role who recorded this payment (from the Payment entity's own
    // "Created" audit row) — not always a Komendant, could be a Xəzinədar. Null when
    // that audit row's actor can't be resolved (e.g. a legacy payment).
    string? IssuerName,
    string? IssuerTitle,
    // The complex's building address — resolved from the Apartment/Building shadow for
    // apartment payments, or the complex's one Building for garage/unattached payments.
    string? BuildingAddress);
