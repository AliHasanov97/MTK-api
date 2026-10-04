using MTK.Common.Application.Exporting;
using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Payments.Queries.ExportPaymentReceipt;

/// <summary>
/// The frontend only ever sends PaymentId — every other field on the receipt (payer
/// name, apartment/garage number, billing period, building address, issuer name/role)
/// is resolved server-side from Payments' own data. See ExportPaymentReceiptQueryHandler.
/// </summary>
public sealed record ExportPaymentReceiptQuery(Guid PaymentId) : IQuery<ExportFileResult>;
