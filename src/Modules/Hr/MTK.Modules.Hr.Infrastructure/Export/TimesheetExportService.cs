using ClosedXML.Excel;
using MTK.Modules.Hr.Application.Timesheets.ExportTimesheet;
using MTK.Modules.Hr.Application.Timesheets.GetTimesheets;

namespace MTK.Modules.Hr.Infrastructure.Export;

internal sealed class TimesheetExportService : ITimesheetExportService
{
    private static readonly string[] MonthNames =
    {
        "Yanvar", "Fevral", "Mart", "Aprel", "May", "İyun",
        "İyul", "Avqust", "Sentyabr", "Oktyabr", "Noyabr", "Dekabr"
    };

    public Task<(MemoryStream Stream, string FileName)> ExportToExcelAsync(
        GetTimesheetsResponse timesheetData,
        string companyName,
        string? directorName,
        CancellationToken cancellationToken = default)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Tabel");

        var monthName = MonthNames[timesheetData.Month - 1];
        int daysInMonth = timesheetData.DaysInMonth;

        // Sütun sayı: 4 sabit + günlər + 12 summary = 4 + 28 + 12 = 44 (max)
        int daysStartCol = 5;  // E sütunu
        int summaryStartCol = daysStartCol + daysInMonth; // AG sütunu (28 gün üçün = 33)
        int totalColumns = summaryStartCol + 12 - 1; // AR sütunu = 44

        SetupHeader(ws, companyName, directorName, timesheetData.Year, monthName, daysInMonth, summaryStartCol, totalColumns);
        int lastDataRow = SetupDataTable(ws, timesheetData, summaryStartCol);
        AddLegendTable(ws, lastDataRow + 2);
        SetupPageSettings(ws);

        var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        var fileName = $"Tabel_{timesheetData.Year}_{timesheetData.Month:D2}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";

        return Task.FromResult((stream, fileName));
    }

    private void SetupHeader(IXLWorksheet ws, string companyName, string? directorName,
        int year, string monthName, int daysInMonth, int summaryStartCol, int totalColumns)
    {
        // ── Row 1: Şirkət direktoru + imza + direktor adı + tarix ──────
        // Bütün Row 1 bir sətirdə: "CompanyName"-nin direktoru ______________ Direktor Adı "___" __________ 2026-ci il.
        string directorText = directorName ?? "_________________";
        string row1Text = $"\"{companyName}\"-nin direktoru                    ______________                    {directorText}                    \"___\" __________ {year}-ci il.";

        ws.Cell(1, 1).Value = row1Text;
        ws.Range(1, 1, 1, totalColumns).Merge();
        ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        ws.Cell(1, 1).Style.Font.Bold = true;
        ws.Cell(1, 1).Style.Font.FontSize = 10;
        ws.Row(1).Height = 25;

        // ── Row 2: Ay başlığı ────────────────────────────────────────────
        ws.Cell(2, 1).Value = $"{year}-ci ilin {monthName}  ayı üzrə";
        ws.Range(2, 1, 2, totalColumns).Merge();
        ws.Cell(2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Cell(2, 1).Style.Font.Bold = true;
        ws.Cell(2, 1).Style.Font.FontSize = 12;
        ws.Row(2).Height = 20;

        // ── Row 3: Əsas başlıq + Summary sütun başlıqları ────────────────
        ws.Cell(3, 1).Value = "İ Ş   V A X T I N I N   U Ç O T   T A B E L İ";
        ws.Range(3, 1, 3, summaryStartCol - 1).Merge();
        ws.Cell(3, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Cell(3, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        ws.Cell(3, 1).Style.Font.Bold = true;
        ws.Cell(3, 1).Style.Font.FontSize = 13;

        // Summary sütun başlıqları (row 3-5 merge)
        string[] summaryHeaders = {
            "Aylıq faktiki iş saatı",
            "Aylıq faktiki iş günləri",
            "Aylıq iş saatı norma",
            "Aylıq iş günləri norma",
            "Əmək Məzuniyyət",
            "Sosial məzuniyyət",
            "Təhsil məzuniyyəti",
            "Ödənişsiz məzuniyyət",
            "Xəstəlik vərəqəsi",
            "Ezamiyyət",
            "Bayram/ Hüzün Günü",
            "Qeyri iş günü"
        };

        for (int i = 0; i < summaryHeaders.Length; i++)
        {
            int col = summaryStartCol + i;
            ws.Range(3, col, 5, col).Merge();
            ws.Cell(3, col).Value = summaryHeaders[i];
            ws.Cell(3, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(3, col).Style.Alignment.Vertical = XLAlignmentVerticalValues.Bottom;
            ws.Cell(3, col).Style.Alignment.WrapText = true;
            ws.Cell(3, col).Style.Alignment.TextRotation = 90; // Rotate up
            ws.Cell(3, col).Style.Font.Bold = true;
            ws.Cell(3, col).Style.Font.FontSize = 9;
            ws.Cell(3, col).Style.Fill.BackgroundColor = XLColor.LightCyan;
        }
        ws.Row(3).Height = 50;

        // ── Row 4: Sütun başlıqları ──────────────────────────────────────
        ws.Range(4, 1, 5, 1).Merge();
        ws.Cell(4, 1).Value = "№";

        ws.Range(4, 2, 5, 2).Merge();
        ws.Cell(4, 2).Value = "Soyadı Adı Ata adı";

        ws.Range(4, 3, 5, 3).Merge();
        ws.Cell(4, 3).Value = "Tabel №";

        ws.Range(4, 4, 5, 4).Merge();
        ws.Cell(4, 4).Value = "Vəzifəsi";

        ws.Range(4, 5, 4, summaryStartCol - 1).Merge();
        ws.Cell(4, 5).Value = "T Ə Q V İ M   A Y I N   G Ü N L Ə R İ";

        // Header hüceyrə stilləri
        foreach (int col in new[] { 1, 2, 3, 4, 5 })
        {
            ws.Cell(4, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(4, col).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell(4, col).Style.Alignment.WrapText = true;
            ws.Cell(4, col).Style.Font.Bold = true;
            ws.Cell(4, col).Style.Font.FontSize = 10;
            ws.Cell(4, col).Style.Fill.BackgroundColor = XLColor.LightYellow;
        }
        ws.Row(4).Height = 30;

        // ── Row 5: Gün nömrələri ─────────────────────────────────────────
        for (int d = 1; d <= daysInMonth; d++)
        {
            var cell = ws.Cell(5, 4 + d);
            cell.Value = d;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontSize = 9;
            cell.Style.Fill.BackgroundColor = XLColor.LightYellow;
        }
        ws.Row(5).Height = 18;

        // ── Sütun genişlikləri ───────────────────────────────────────────
        ws.Column(1).Width = 4;   // №
        ws.Column(2).Width = 28;  // Ad
        ws.Column(3).Width = 12;  // Tabel nömrəsi
        ws.Column(4).Width = 22;  // Vəzifə

        // Gün sütunları - sabit ölçü
        for (int c = 5; c < summaryStartCol; c++)
        {
            ws.Column(c).Width = 3.5;
        }

        // Summary sütunları - minimum ölçü (rotation olduğu üçün)
        for (int c = summaryStartCol; c <= totalColumns; c++)
        {
            ws.Column(c).Width = 4;
        }

        // ── Header sərhədləri ────────────────────────────────────────────
        ws.Range(3, 1, 5, totalColumns).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
        ws.Range(3, 1, 5, totalColumns).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
    }

    private int SetupDataTable(IXLWorksheet ws, GetTimesheetsResponse data, int summaryStartCol)
    {
        int startRow = 6;
        int currentRow = startRow;
        int serialNumber = 1;

        foreach (var employee in data.Employees)
        {
            int r = currentRow;

            ws.Cell(r, 1).Value = serialNumber++;
            ws.Cell(r, 2).Value = employee.Employee.Name;
            ws.Cell(r, 3).Value = employee.RegisterNumber;
            ws.Cell(r, 4).Value = employee.Position?.Name ?? "";

            // Günlər - E sütunundan başlayır (col 5)
            int col = 5;
            foreach (var day in employee.Days.OrderBy(d => d.Day))
            {
                var cell = ws.Cell(r, col);
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Font.FontSize = 9;

                if (day.DayCode == "İ" && day.WorkedHours.HasValue)
                {
                    cell.Value = day.WorkedHours.Value;
                }
                else if (day.DayCode == "Q" || day.DayCode == "-")
                {
                    cell.Value = "İ";
                    cell.Style.Font.FontColor = XLColor.Red;
                }
                else if (day.DayCode == "B" || day.DayCode == "H")
                {
                    cell.Value = "İ";
                    cell.Style.Font.FontColor = XLColor.Red;
                    cell.Style.Fill.BackgroundColor = XLColor.LightYellow;
                }
                else
                {
                    cell.Value = day.DayCode;
                    switch (day.DayCode)
                    {
                        case "M":
                            cell.Style.Fill.BackgroundColor = XLColor.LightGreen;
                            break;
                        case "Y":
                            cell.Style.Fill.BackgroundColor = XLColor.LightBlue;
                            break;
                        case "TM":
                            cell.Style.Fill.BackgroundColor = XLColor.LightCyan;
                            break;
                        case "ÖM":
                            cell.Style.Fill.BackgroundColor = XLColor.LightYellow;
                            break;
                        case "X":
                            cell.Style.Fill.BackgroundColor = XLColor.PeachPuff;
                            break;
                        case "E":
                            cell.Style.Fill.BackgroundColor = XLColor.Lavender;
                            break;
                        case "İG":
                            cell.Style.Font.FontColor = XLColor.Red;
                            cell.Style.Fill.BackgroundColor = XLColor.LightPink;
                            break;
                    }
                }
                col++;
            }

            // Summary sütunları
            int sc = summaryStartCol;
            ws.Cell(r, sc++).Value = employee.ActualWorkedHours;      // Aylıq faktiki iş saatı
            ws.Cell(r, sc++).Value = employee.ActualWorkingDays;      // Aylıq faktiki iş günləri
            ws.Cell(r, sc++).Value = employee.NormWorkedHours;        // Aylıq iş saatı norma
            ws.Cell(r, sc++).Value = employee.NormWorkingDays;        // Aylıq iş günləri norma
            ws.Cell(r, sc++).Value = employee.AnnualLeaveDays;        // Əmək Məzuniyyət
            ws.Cell(r, sc++).Value = employee.SocialLeaveDays;        // Sosial məzuniyyət
            ws.Cell(r, sc++).Value = employee.EducationLeaveDays;     // Təhsil məzuniyyəti
            ws.Cell(r, sc++).Value = employee.UnpaidLeaveDays;        // Ödənişsiz məzuniyyət
            ws.Cell(r, sc++).Value = employee.SickLeaveDays;          // Xəstəlik vərəqəsi
            ws.Cell(r, sc++).Value = employee.BusinessTripDays;       // Ezamiyyət
            ws.Cell(r, sc++).Value = employee.HolidayDays;            // Bayram/Hüzün Günü
            ws.Cell(r, sc++).Value = employee.NonWorkingDays;         // Qeyri iş günü

            // Sıra stilləri
            ws.Row(r).Height = 18;
            for (int c = 1; c < sc; c++)
            {
                var cell = ws.Cell(r, c);
                cell.Style.Font.FontSize = 9;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }
            ws.Cell(r, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(r, 3).Style.Alignment.WrapText = true;
            ws.Cell(r, 4).Style.Alignment.WrapText = true;

            // Alternativ sıra rəngi
            if (serialNumber % 2 == 0)
            {
                ws.Range(r, 1, r, sc - 1).Style.Fill.BackgroundColor = XLColor.FromArgb(240, 248, 255);
            }

            currentRow++;
        }

        return currentRow - 1;
    }

    private void AddLegendTable(IXLWorksheet ws, int startRow)
    {
        var legendData = new[]
        {
            ("Əmək məzuniyyəti", "M", XLColor.LightGreen),
            ("Sosial məzuniyyət", "Y", XLColor.LightBlue),
            ("Təhsil məzuniyyəti", "TM", XLColor.LightCyan),
            ("Ödənişsiz məzuniyyət", "ÖM", XLColor.LightYellow),
            ("İşə gəlməmə (üzrsüz)", "İG", XLColor.LightPink),
            ("Əmək qabiliyyətinin olmaması vərəqəsi", "X", XLColor.PeachPuff),
            ("Ezamiyyət", "E", XLColor.Lavender),
            ("Bayram /Hüzün günü", "B / H / S", XLColor.LightYellow),
            ("Qeyri iş günü/İstirahət", "Q / İ", XLColor.White)
        };

        for (int i = 0; i < legendData.Length; i++)
        {
            int row = startRow + i;
            var (description, code, color) = legendData[i];

            // Açıqlama hüceyrəsi
            var descCell = ws.Cell(row, 2);
            descCell.Value = description;
            descCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            descCell.Style.Fill.BackgroundColor = color;

            // Kod hüceyrəsi
            var codeCell = ws.Cell(row, 3);
            codeCell.Value = code;
            codeCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            codeCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            codeCell.Style.Font.Bold = true;

            if (code == "Q / İ")
            {
                codeCell.Style.Font.FontColor = XLColor.Red;
            }
        }
    }

    private void SetupPageSettings(IXLWorksheet ws)
    {
        ws.PageSetup.PageOrientation = XLPageOrientation.Landscape;
        ws.PageSetup.PaperSize = XLPaperSize.A3Paper;
        ws.PageSetup.FitToPages(1, 1);
        ws.PageSetup.Margins.Left = 0.3;
        ws.PageSetup.Margins.Right = 0.3;
        ws.PageSetup.Margins.Top = 0.3;
        ws.PageSetup.Margins.Bottom = 0.3;
    }
}
