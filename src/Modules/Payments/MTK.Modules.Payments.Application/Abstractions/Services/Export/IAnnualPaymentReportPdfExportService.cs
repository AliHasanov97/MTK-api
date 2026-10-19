using MTK.Modules.Payments.Application.Charges.Queries.GetAnnualPaymentReport;

namespace MTK.Modules.Payments.Application.Abstractions.Services.Export;

public interface IAnnualPaymentReportPdfExportService
{
    MemoryStream ExportToPdf(
        AnnualPaymentReportResponse report,
        IReadOnlyDictionary<Guid, PropertyExportLabel> labels,
        string companyName);
}
