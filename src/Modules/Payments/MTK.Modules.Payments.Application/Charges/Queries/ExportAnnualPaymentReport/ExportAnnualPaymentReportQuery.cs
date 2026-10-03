using MTK.Common.Application.Exporting;
using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Domain.Charges;

namespace MTK.Modules.Payments.Application.Charges.Queries.ExportAnnualPaymentReport;

public sealed record ExportAnnualPaymentReportQuery(int Year, PropertyType? PropertyType)
    : IQuery<ExportFileResult>;
