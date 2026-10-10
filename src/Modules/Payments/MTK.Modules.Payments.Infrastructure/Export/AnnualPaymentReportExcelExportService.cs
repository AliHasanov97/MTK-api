using ClosedXML.Excel;
using MTK.Modules.Payments.Application.Abstractions.Services.Export;
using MTK.Modules.Payments.Application.Charges.Queries.GetAnnualPaymentReport;
using MTK.Modules.Payments.Domain.Charges;

namespace MTK.Modules.Payments.Infrastructure.Export;

/// <summary>
/// Bütün mənzil və qarajların illik ödəniş qrafiki. Düzülüş iş vaxtı tabeli ilə eynidir:
/// başlıq sətirləri, sabit sütunlar, ay xanaları, yekun sütunları və şərti işarələr cədvəli.
/// </summary>
internal sealed class AnnualPaymentReportExcelExportService : IAnnualPaymentReportExcelExportService
{
    private static readonly string[] MonthNames =
    [
        "Yan", "Fev", "Mar", "Apr", "May", "İyn",
        "İyl", "Avq", "Sen", "Okt", "Noy", "Dek",
    ];

    // Sabit sütunlar: № | Növ | Nömrə | Bina | Sahibi
    private const int FixedColumns = 5;
    private const int MonthStartCol = FixedColumns + 1;           // 6
    private const int SummaryStartCol = MonthStartCol + 12;       // 18
    private const int TotalColumns = SummaryStartCol + 2;         // 20 (Hesablanıb / Ödənilib / Cari borc)
    private const int HeaderRows = 4;

    public MemoryStream ExportToExcel(AnnualPaymentReportResponse report, IReadOnlyDictionary<Guid, PropertyExportLabel> labels)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add($"{report.Year}");

        SetupHeader(ws, report.Year);
        var lastRow = FillRows(ws, report, labels);
        AddLegendTable(ws, lastRow + 2);
        SetupPageSettings(ws);

        var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;
        return stream;
    }

    private static void SetupHeader(IXLWorksheet ws, int year)
    {
        // Row 1: il
        ws.Cell(1, 1).Value = $"{year}-ci il üzrə";
        ws.Range(1, 1, 1, TotalColumns).Merge();
        ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Cell(1, 1).Style.Font.Bold = true;
        ws.Cell(1, 1).Style.Font.FontSize = 12;
        ws.Row(1).Height = 20;

        // Row 2: əsas başlıq
        ws.Cell(2, 1).Value = "M Ə N Z İ L   V Ə   Q A R A J L A R I N   İ L L İ K   Ö D Ə N İ Ş   Q R A F İ K İ";
        ws.Range(2, 1, 2, TotalColumns).Merge();
        ws.Cell(2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Cell(2, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        ws.Cell(2, 1).Style.Font.Bold = true;
        ws.Cell(2, 1).Style.Font.FontSize = 13;
        ws.Row(2).Height = 26;

        // Row 3-4: sütun başlıqları
        string[] fixedHeaders = ["№", "Növ", "Nömrə", "Bina", "Sahibi"];
        for (var i = 0; i < fixedHeaders.Length; i++)
        {
            ws.Range(3, 1 + i, 4, 1 + i).Merge();
            ws.Cell(3, 1 + i).Value = fixedHeaders[i];
        }

        ws.Range(3, MonthStartCol, 3, MonthStartCol + 11).Merge();
        ws.Cell(3, MonthStartCol).Value = "A Y L A R";

        for (var month = 1; month <= 12; month++)
        {
            ws.Cell(4, MonthStartCol + month - 1).Value = MonthNames[month - 1];
        }

        string[] summaryHeaders = ["Hesablanıb (₼)", "Ödənilib (₼)", "Cari borc (₼)"];
        for (var i = 0; i < summaryHeaders.Length; i++)
        {
            ws.Range(3, SummaryStartCol + i, 4, SummaryStartCol + i).Merge();
            ws.Cell(3, SummaryStartCol + i).Value = summaryHeaders[i];
        }

        var head = ws.Range(3, 1, HeaderRows, TotalColumns);
        head.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        head.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        head.Style.Alignment.WrapText = true;
        head.Style.Font.Bold = true;
        head.Style.Font.FontSize = 10;
        head.Style.Fill.BackgroundColor = XLColor.LightYellow;
        head.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
        head.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        ws.Range(3, SummaryStartCol, 4, TotalColumns).Style.Fill.BackgroundColor = XLColor.LightCyan;
        ws.Row(3).Height = 22;
        ws.Row(4).Height = 20;

        ws.Column(1).Width = 5;    // №
        ws.Column(2).Width = 9;    // Növ
        ws.Column(3).Width = 10;   // Nömrə
        ws.Column(4).Width = 22;   // Bina
        ws.Column(5).Width = 30;   // Sahibi
        for (var col = MonthStartCol; col < SummaryStartCol; col++)
        {
            ws.Column(col).Width = 5.5;
        }
        for (var col = SummaryStartCol; col <= TotalColumns; col++)
        {
            ws.Column(col).Width = 14;
        }

        ws.SheetView.FreezeRows(HeaderRows);
        ws.SheetView.FreezeColumns(FixedColumns);
    }

    private static int FillRows(
        IXLWorksheet ws,
        AnnualPaymentReportResponse report,
        IReadOnlyDictionary<Guid, PropertyExportLabel> labels)
    {
        var row = HeaderRows + 1;
        var serial = 1;
        decimal totalAmount = 0, totalPaid = 0, totalDebt = 0;

        var ordered = report.Properties
            .Select(p => (Property: p, Label: labels.GetValueOrDefault(p.PropertyId)))
            .OrderBy(x => x.Property.PropertyType == PropertyType.Garage ? 1 : 0)
            .ThenBy(x => x.Label?.Building, StringComparer.CurrentCultureIgnoreCase)
            .ThenBy(x => NumberSortKey(x.Label?.Number))
            .ThenBy(x => x.Label?.Number, StringComparer.CurrentCultureIgnoreCase);

        foreach (var (property, label) in ordered)
        {
            ws.Cell(row, 1).Value = serial;
            ws.Cell(row, 2).Value = property.PropertyType == PropertyType.Garage ? "Qaraj" : "Mənzil";
            ws.Cell(row, 3).Value = label?.Number ?? "—";
            ws.Cell(row, 4).Value = label?.Building ?? "";
            ws.Cell(row, 5).Value = label?.Owner ?? "";

            foreach (var month in property.Months)
            {
                var cell = ws.Cell(row, MonthStartCol + month.Month - 1);

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

                    // Statik fayl üçün "hover": dəqiq məbləğlər
                    cell.GetComment().AddText($"Haqq: {month.Amount:N2} ₼\nÖdənilib: {month.PaidAmount:N2} ₼");
                }
            }

            var amount = property.Months.Sum(m => m.Amount);
            var paid = property.Months.Sum(m => m.PaidAmount);
            totalAmount += amount;
            totalPaid += paid;
            totalDebt += property.CurrentDebt;

            ws.Cell(row, SummaryStartCol).Value = amount;
            ws.Cell(row, SummaryStartCol + 1).Value = paid;
            var debtCell = ws.Cell(row, SummaryStartCol + 2);
            debtCell.Value = property.CurrentDebt;
            if (property.CurrentDebt > 0)
            {
                debtCell.Style.Font.FontColor = XLColor.DarkRed;
                debtCell.Style.Font.Bold = true;
            }

            var line = ws.Range(row, 1, row, TotalColumns);
            line.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            line.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            ws.Range(row, 1, row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range(row, MonthStartCol, row, SummaryStartCol - 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range(row, SummaryStartCol, row, TotalColumns).Style.NumberFormat.Format = "#,##0.00";

            if (serial % 2 == 0)
            {
                ws.Range(row, 1, row, FixedColumns).Style.Fill.BackgroundColor = XLColor.FromArgb(240, 248, 255);
            }

            serial++;
            row++;
        }

        // Yekun sətri
        ws.Range(row, 1, row, SummaryStartCol - 1).Merge();
        ws.Cell(row, 1).Value = "YEKUN";
        ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        ws.Cell(row, SummaryStartCol).Value = totalAmount;
        ws.Cell(row, SummaryStartCol + 1).Value = totalPaid;
        ws.Cell(row, SummaryStartCol + 2).Value = totalDebt;
        var total = ws.Range(row, 1, row, TotalColumns);
        total.Style.Font.Bold = true;
        total.Style.Fill.BackgroundColor = XLColor.LightYellow;
        total.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
        total.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        ws.Range(row, SummaryStartCol, row, TotalColumns).Style.NumberFormat.Format = "#,##0.00";

        return row;
    }

    private static void AddLegendTable(IXLWorksheet ws, int startRow)
    {
        var legend = new[]
        {
            ("Tam ödənilib", "+", XLColor.LightGreen),
            ("Qismən ödənilib", "±", XLColor.LightYellow),
            ("Ödənilməyib (borc)", "−", XLColor.LightPink),
            ("Həmin ay hesablama yoxdur", "—", XLColor.White),
        };

        for (var i = 0; i < legend.Length; i++)
        {
            var (description, code, color) = legend[i];
            var row = startRow + i;

            ws.Range(row, 2, row, 5).Merge();
            var descCell = ws.Cell(row, 2);
            descCell.Value = description;
            descCell.Style.Fill.BackgroundColor = color;
            ws.Range(row, 2, row, 5).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            var codeCell = ws.Cell(row, MonthStartCol);
            codeCell.Value = code;
            codeCell.Style.Fill.BackgroundColor = color;
            codeCell.Style.Font.Bold = true;
            codeCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            codeCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        }
    }

    private static void SetupPageSettings(IXLWorksheet ws)
    {
        ws.PageSetup.PageOrientation = XLPageOrientation.Landscape;
        ws.PageSetup.PaperSize = XLPaperSize.A3Paper;
        ws.PageSetup.FitToPages(1, 0);
        ws.PageSetup.SetRowsToRepeatAtTop(1, HeaderRows);
        ws.PageSetup.Margins.Left = 0.3;
        ws.PageSetup.Margins.Right = 0.3;
        ws.PageSetup.Margins.Top = 0.3;
        ws.PageSetup.Margins.Bottom = 0.3;
    }

    /// <summary>"12" → 12, "12A" → 12, nömrəsiz → böyük ədəd (sona düşür)</summary>
    private static int NumberSortKey(string? number)
    {
        if (string.IsNullOrEmpty(number)) return int.MaxValue;
        var digits = new string(number.TakeWhile(char.IsDigit).ToArray());
        return int.TryParse(digits, out var n) ? n : int.MaxValue;
    }
}
