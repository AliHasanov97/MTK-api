using MTK.Modules.Payments.Domain.Parties;
using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Application.Abstractions.Services.Export;

public interface IPaymentReceiptExportService
{
    MemoryStream ExportToPdf(PaymentReceiptData data);
}

/// <summary>One line in the receipt's charge breakdown — which charge this payment covered and how much of it.</summary>
public sealed record PaymentReceiptLine(string Description, decimal Amount);

/// <summary>
/// Everything the PDF needs, already resolved. PayerName/PropertyLabel come from the
/// caller (frontend) rather than a cross-module lookup — Payments has no Owner name of
/// its own (see PropertyOwnership — only an OwnerId), and the frontend already has
/// these labels on screen for the payment it's requesting a receipt for.
/// </summary>
public sealed record PaymentReceiptData(
    Guid PaymentId,
    decimal Amount,
    PaymentMethod PaymentMethod,
    DateTimeOffset PaymentDate,
    string? Notes,
    PartyType PartyType,
    string? PayerName,
    string? PropertyLabel,
    IReadOnlyCollection<PaymentReceiptLine> Lines);
