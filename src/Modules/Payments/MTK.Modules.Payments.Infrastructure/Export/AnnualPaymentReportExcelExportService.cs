using ClosedXML.Excel;
using MTK.Modules.Payments.Application.Abstractions.Services.Export;
using MTK.Modules.Payments.Application.Charges.Queries.GetAnnualPaymentReport;
using MTK.Modules.Payments.Domain.Charges;

namespace MTK.Modules.Payments.Infrastructure.Export;

internal sealed class AnnualPaymentReportExcelExportService : IAnnualPaymentReportExcelExportService
{
    private static readonly string[] MonthNames =
    [
        "Yan", "Fev", "Mar", "Apr", "May", "İyn",
        "İyl", "Avq", "Sen", "Okt", "Noy", "Dek",
    ];

    public MemoryStream ExportToExcel(AnnualPaymentReportResponse report, IReadOnlyDictionary<Guid, string> propertyLabels)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add($"{report.Year}");

        SetupHeader(ws);
        FillRows(ws, report, propertyLabels);
        SetupPageSettings(ws);

        var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;
        return stream;
    }

    private void SetupHeader(IXLWorksheet ws)
    {
        ws.Cell(1, 1).Value = "Əmlak";
        for (var month = 1; month <= 12; month++)
        {
            ws.Cell(1, 1 + month).Value = MonthNames[month - 1];
        }
        ws.Cell(1, 14).Value = "Cari borc (₼)";

        var headerRange = ws.Range(1, 1, 1, 14);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightYellow;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        ws.Column(1).Width = 24;
        for (var col = 2; col <= 13; col++)
        {
            ws.Column(col).Width = 9;
        }
        ws.Column(14).Width = 14;

        ws.SheetView.FreezeRows(1);
    }

    private void FillRows(IXLWorksheet ws, AnnualPaymentReportResponse report, IReadOnlyDictionary<Guid, string> propertyLabels)
    {
        var row = 2;
        foreach (var property in report.Properties)
        {
            ws.Cell(row, 1).Value = propertyLabels.GetValueOrDefault(
                property.PropertyId,
                property.PropertyType == PropertyType.Garage ? "Qaraj" : "Mənzil");

            foreach (var month in property.Months)
            {
                var cell = ws.Cell(row, 1 + month.Month);

                if (month.Status is null)
                {
                    cell.Value = "—";
                    cell.Style.Font.FontColor = XLColor.Gray;
                }
                else
                {
                    cell.Value = month.Status switch
                    {
                        ChargeStatus.Paid => "+",
                        ChargeStatus.PartiallyPaid => "±",
                        _ => "−",
                    };
                    cell.Style.Fill.BackgroundColor = month.Status switch
                    {
                        ChargeStatus.Paid => XLColor.LightGreen,
                        ChargeStatus.PartiallyPaid => XLColor.LightYellow,
                        _ => XLColor.LightPink,
                    };
                    cell.Style.Font.Bold = true;
                }

                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                // Hover-equivalent for a static file: the exact amounts, since the
                // grid cell itself only has room for one status glyph.
                cell.GetComment().AddText($"Haqq: {month.Amount:N2} ₼\nÖdənilib: {month.PaidAmount:N2} ₼");
            }

            var debtCell = ws.Cell(row, 14);
            debtCell.Value = property.CurrentDebt;
            debtCell.Style.NumberFormat.Format = "#,##0.00";
            debtCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            if (property.CurrentDebt > 0)
            {
                debtCell.Style.Font.FontColor = XLColor.DarkRed;
                debtCell.Style.Font.Bold = true;
            }

            ws.Cell(row, 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            debtCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            row++;
        }
    }

    private void SetupPageSettings(IXLWorksheet ws)
    {
        ws.PageSetup.PageOrientation = XLPageOrientation.Landscape;
        ws.PageSetup.PaperSize = XLPaperSize.A4Paper;
        ws.PageSetup.FitToPages(1, 0);
    }
}
