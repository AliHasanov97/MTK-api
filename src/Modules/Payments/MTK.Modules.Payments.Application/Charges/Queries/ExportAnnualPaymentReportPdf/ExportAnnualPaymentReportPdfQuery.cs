using MTK.Common.Application.Exporting;
using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Domain.Charges;

namespace MTK.Modules.Payments.Application.Charges.Queries.ExportAnnualPaymentReportPdf;

public sealed record ExportAnnualPaymentReportPdfQuery(int Year, PropertyType? PropertyType)
    : IQuery<ExportFileResult>;
