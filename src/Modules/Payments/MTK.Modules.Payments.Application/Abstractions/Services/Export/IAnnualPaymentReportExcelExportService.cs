using MTK.Modules.Payments.Application.Charges.Queries.GetAnnualPaymentReport;

namespace MTK.Modules.Payments.Application.Abstractions.Services.Export;

/// <summary>Excel sətri üçün əmlakın təsviri.</summary>
/// <param name="Number">Mənzil/qaraj nömrəsi</param>
/// <param name="Building">Bina adı (qaraj üçün null)</param>
/// <param name="Owner">Sahibin tam adı</param>
public sealed record PropertyExportLabel(string Number, string? Building, string? Owner);

public interface IAnnualPaymentReportExcelExportService
{
    /// <param name="labels">PropertyId -> Excel sətri üçün nömrə / bina / sahib, Payments-in öz PropertyOwnership oxu modelindən.</param>
    MemoryStream ExportToExcel(AnnualPaymentReportResponse report, IReadOnlyDictionary<Guid, PropertyExportLabel> labels);
}
