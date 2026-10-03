using MTK.Common.Application.Exporting;
using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Payments.Queries.ExportPaymentReceipt;

/// <summary>
/// PayerName/PropertyLabel are optional display-only overrides — the frontend already
/// resolved them (it has the owner/vendor name on screen) and passes them along so the
/// receipt doesn't just show a bare GUID. Missing them degrades gracefully (see handler).
/// </summary>
public sealed record ExportPaymentReceiptQuery(Guid PaymentId, string? PayerName, string? PropertyLabel)
    : IQuery<ExportFileResult>;
