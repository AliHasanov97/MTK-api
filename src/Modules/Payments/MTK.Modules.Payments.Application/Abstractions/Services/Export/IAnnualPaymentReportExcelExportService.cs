using MTK.Modules.Payments.Application.Charges.Queries.GetAnnualPaymentReport;

namespace MTK.Modules.Payments.Application.Abstractions.Services.Export;

public interface IAnnualPaymentReportExcelExportService
{
    /// <param name="propertyLabels">PropertyId -> display label (e.g. "290 (Mənzil)"), resolved from Payments' own PropertyOwnership read-model.</param>
    MemoryStream ExportToExcel(AnnualPaymentReportResponse report, IReadOnlyDictionary<Guid, string> propertyLabels);
}
