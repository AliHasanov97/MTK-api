using ClosedXML.Excel;
using MTK.Modules.Payments.Application.Abstractions.Services.Export;
using MTK.Modules.Payments.Application.Charges.Queries.GetAnnualPaymentReport;
using MTK.Modules.Payments.Domain.Charges;

namespace MTK.Modules.Payments.Infrastructure.Export;

/// <summary>
/// Mənzil və/və ya qarajların illik ödəniş qrafiki. Düzülüş iş vaxtı tabeli ilə eynidir:
/// başlıq sətirləri, sabit sütunlar, ay xanaları, yekun sütunları və şərti işarələr cədvəli.
/// Başlıq və sütunlar hesabatın tərkibinə uyğun dəyişir: yalnız mənzillər, yalnız qarajlar və ya hər ikisi.
/// </summary>
internal sealed class AnnualPaymentReportExcelExportService : IAnnualPaymentReportExcelExportService
{
    private static readonly string[] MonthNames =
    [
        "Yan", "Fev", "Mar", "Apr", "May", "İyn",
        "İyl", "Avq", "Sen", "Okt", "Noy", "Dek",
    ];

    private const int HeaderRows = 4;

    /// <summary>Hesabatın tərkibinə görə sütun düzülüşü və başlıq.</summary>
    private sealed record Layout(bool ShowKind, bool ShowBuilding, string Title, string SheetSuffix)
    {
        // № | [Növ] | Nömrə | [Bina] | Sahibi
        public int KindCol => 2;
        public int NumberCol => ShowKind ? 3 : 2;
        public int BuildingCol => NumberCol + 1;
        public int OwnerCol => ShowBuilding ? BuildingCol + 1 : NumberCol + 1;
        public int FixedColumns => OwnerCol;
        public int MonthStartCol => FixedColumns + 1;
        public int SummaryStartCol => MonthStartCol + 12;
        public int TotalColumns => SummaryStartCol + 2; // Hesablanıb / Ödənilib / Cari borc
    }

    private static Layout CreateLayout(AnnualPaymentReportResponse report)
    {
        var hasApartments = report.Properties.Any(p => p.PropertyType == PropertyType.Apartment);
        var hasGarages = report.Properties.Any(p => p.PropertyType == PropertyType.Garage);

        if (hasGarages && !hasApartments)
            return new Layout(false, false, "Q A R A J L A R I N   İ L L İ K   Ö D Ə N İ Ş   Q R A F İ K İ", "Qarajlar");

        if (hasApartments && !hasGarages)
            return new Layout(false, true, "M Ə N Z İ L L Ə R İ N   İ L L İ K   Ö D Ə N İ Ş   Q R A F İ K İ", "Mənzillər");

        return new Layout(true, true, "M Ə N Z İ L   V Ə   Q A R A J L A R I N   İ L L İ K   Ö D Ə N İ Ş   Q R A F İ K İ", "Mənzil və qarajlar");
    }

    public MemoryStream ExportToExcel(AnnualPaymentReportResponse report, IReadOnlyDictionary<Guid, PropertyExportLabel> labels)
    {
        var layout = CreateLayout(report);

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add($"{report.Year}");

        SetupHeader(ws, layout, report.Year);
        var lastRow = FillRows(ws, layout, report, labels);
        AddLegendTable(ws, layout, lastRow + 2);
        SetupPageSettings(ws);

        var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;
        return stream;
    }

    private static void SetupHeader(IXLWorksheet ws, Layout l, int year)
    {
        // Row 1: il
        ws.Cell(1, 1).Value = $"{year}-ci il üzrə";
        ws.Range(1, 1, 1, l.TotalColumns).Merge();
        ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Cell(1, 1).Style.Font.Bold = true;
        ws.Cell(1, 1).Style.Font.FontSize = 12;
        ws.Row(1).Height = 20;

        // Row 2: əsas başlıq
        ws.Cell(2, 1).Value = l.Title;
        ws.Range(2, 1, 2, l.TotalColumns).Merge();
        ws.Cell(2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Cell(2, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        ws.Cell(2, 1).Style.Font.Bold = true;
        ws.Cell(2, 1).Style.Font.FontSize = 13;
        ws.Row(2).Height = 26;

        // Row 3-4: sütun başlıqları
        var fixedHeaders = new List<(int Col, string Text)> { (1, "№") };
        if (l.ShowKind) fixedHeaders.Add((l.KindCol, "Növ"));
        fixedHeaders.Add((l.NumberCol, "Nömrə"));
        if (l.ShowBuilding) fixedHeaders.Add((l.BuildingCol, "Bina"));
        fixedHeaders.Add((l.OwnerCol, "Sahibi"));

        foreach (var (col, text) in fixedHeaders)
        {
            ws.Range(3, col, 4, col).Merge();
            ws.Cell(3, col).Value = text;
        }

        ws.Range(3, l.MonthStartCol, 3, l.MonthStartCol + 11).Merge();
        ws.Cell(3, l.MonthStartCol).Value = "A Y L A R";

        for (var month = 1; month <= 12; month++)
        {
            ws.Cell(4, l.MonthStartCol + month - 1).Value = MonthNames[month - 1];
        }

        string[] summaryHeaders = ["Hesablanıb (₼)", "Ödənilib (₼)", "Cari borc (₼)"];
        for (var i = 0; i < summaryHeaders.Length; i++)
        {
            ws.Range(3, l.SummaryStartCol + i, 4, l.SummaryStartCol + i).Merge();
            ws.Cell(3, l.SummaryStartCol + i).Value = summaryHeaders[i];
        }

        var head = ws.Range(3, 1, HeaderRows, l.TotalColumns);
        head.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        head.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        head.Style.Alignment.WrapText = true;
        head.Style.Font.Bold = true;
        head.Style.Font.FontSize = 10;
        head.Style.Fill.BackgroundColor = XLColor.LightYellow;
        head.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
        head.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        ws.Range(3, l.SummaryStartCol, 4, l.TotalColumns).Style.Fill.BackgroundColor = XLColor.LightCyan;
        ws.Row(3).Height = 22;
        ws.Row(4).Height = 20;

        ws.Column(1).Width = 5;                      // №
        if (l.ShowKind) ws.Column(l.KindCol).Width = 9;
        ws.Column(l.NumberCol).Width = 10;           // Nömrə
        if (l.ShowBuilding) ws.Column(l.BuildingCol).Width = 22;
        ws.Column(l.OwnerCol).Width = 30;            // Sahibi
        for (var col = l.MonthStartCol; col < l.SummaryStartCol; col++)
        {
            ws.Column(col).Width = 5.5;
        }
        for (var col = l.SummaryStartCol; col <= l.TotalColumns; col++)
        {
            ws.Column(col).Width = 14;
        }

        ws.SheetView.FreezeRows(HeaderRows);
        ws.SheetView.FreezeColumns(l.FixedColumns);
    }

    private static int FillRows(
        IXLWorksheet ws,
        Layout l,
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
            if (l.ShowKind)
                ws.Cell(row, l.KindCol).Value = property.PropertyType == PropertyType.Garage ? "Qaraj" : "Mənzil";
            ws.Cell(row, l.NumberCol).Value = label?.Number ?? "—";
            if (l.ShowBuilding)
                ws.Cell(row, l.BuildingCol).Value = label?.Building ?? "";
            ws.Cell(row, l.OwnerCol).Value = label?.Owner ?? "";

            foreach (var month in property.Months)
            {
                var cell = ws.Cell(row, l.MonthStartCol + month.Month - 1);

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

            ws.Cell(row, l.SummaryStartCol).Value = amount;
            ws.Cell(row, l.SummaryStartCol + 1).Value = paid;
            var debtCell = ws.Cell(row, l.SummaryStartCol + 2);
            debtCell.Value = property.CurrentDebt;
            if (property.CurrentDebt > 0)
            {
                debtCell.Style.Font.FontColor = XLColor.DarkRed;
                debtCell.Style.Font.Bold = true;
            }

            var line = ws.Range(row, 1, row, l.TotalColumns);
            line.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            line.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            ws.Range(row, 1, row, l.NumberCol).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range(row, l.MonthStartCol, row, l.SummaryStartCol - 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range(row, l.SummaryStartCol, row, l.TotalColumns).Style.NumberFormat.Format = "#,##0.00";

            if (serial % 2 == 0)
            {
                ws.Range(row, 1, row, l.FixedColumns).Style.Fill.BackgroundColor = XLColor.FromArgb(240, 248, 255);
            }

            serial++;
            row++;
        }

        // Yekun sətri
        ws.Range(row, 1, row, l.SummaryStartCol - 1).Merge();
        ws.Cell(row, 1).Value = "YEKUN";
        ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        ws.Cell(row, l.SummaryStartCol).Value = totalAmount;
        ws.Cell(row, l.SummaryStartCol + 1).Value = totalPaid;
        ws.Cell(row, l.SummaryStartCol + 2).Value = totalDebt;
        var total = ws.Range(row, 1, row, l.TotalColumns);
        total.Style.Font.Bold = true;
        total.Style.Fill.BackgroundColor = XLColor.LightYellow;
        total.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
        total.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        ws.Range(row, l.SummaryStartCol, row, l.TotalColumns).Style.NumberFormat.Format = "#,##0.00";

        return row;
    }

    private static void AddLegendTable(IXLWorksheet ws, Layout l, int startRow)
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

            ws.Range(row, 2, row, l.FixedColumns).Merge();
            var descCell = ws.Cell(row, 2);
            descCell.Value = description;
            descCell.Style.Fill.BackgroundColor = color;
            ws.Range(row, 2, row, l.FixedColumns).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            var codeCell = ws.Cell(row, l.MonthStartCol);
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
