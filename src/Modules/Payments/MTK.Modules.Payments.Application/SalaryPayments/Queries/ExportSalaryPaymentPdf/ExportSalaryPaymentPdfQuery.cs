using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.SalaryPayments.Queries.ExportSalaryPaymentPdf;

/// <summary>
/// Maaş ödənişi PDF-i çıxarış.
/// </summary>
public sealed record ExportSalaryPaymentPdfQuery(Guid SalaryPaymentId) : IQuery<FileExportResult>;

/// <summary>
/// File export nəticəsi.
/// </summary>
public sealed record FileExportResult(
    byte[] Content,
    string ContentType,
    string FileName);
