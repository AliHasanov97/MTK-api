using System.Globalization;
using MTK.Modules.Payments.Application.Abstractions.Services.Export;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MTK.Modules.Payments.Infrastructure.Export;

/// <summary>
/// Əmək haqqı qəbzi (payslip) — PaymentReceiptExportService ilə eyni vizual dil
/// (şirkət adı, başlıq, AccentColor/RuleColor), amma A4 tam səhifə, çünki bu, ödəniş
/// qəbzi deyil, rəsmi aylıq əmək haqqı sənədidir (HR-dəki BonusOrder/SalaryDeduction
/// əmrləri kimi A4 formatında).
/// </summary>
internal sealed class PayslipExportService : IPayslipExportService
{
    private const string Navy = "#0F2A43";
    private const string Accent = "#1D4E89";
    private const string Rule = "#D7E0EA";
    private const string Muted = "#5B6B7C";
    private const string SoftBackground = "#F7F9FC";
    private static readonly string AccentColor = Colors.Blue.Darken2;
    private static readonly string RuleColor = Colors.Grey.Lighten1;
    private static readonly CultureInfo Az = CreateAzCulture();

    private static readonly string[] AzMonths =
    [
        "Yanvar", "Fevral", "Mart", "Aprel", "May", "İyun",
        "İyul", "Avqust", "Sentyabr", "Oktyabr", "Noyabr", "Dekabr",
    ];

    public MemoryStream ExportToPdf(PayslipExportData data)
    {
        var stream = new MemoryStream();

        Document.Create(container => container.Page(page => ComposePayslipPage(page, data)))
            .GeneratePdf(stream);

        return stream;
    }

    public MemoryStream ExportRunToPdf(PayrollRunExportData report)
    {
        var stream = new MemoryStream();

        Document.Create(container => container.Page(page => ComposePayrollReportPage(page, report)))
            .GeneratePdf(stream);

        stream.Position = 0;
        return stream;
    }

    private static void ComposePayrollReportPage(PageDescriptor page, PayrollRunExportData report)
    {
        page.Size(PageSizes.A4.Landscape());
        page.MarginHorizontal(28);
        page.MarginVertical(24);
        page.PageColor(Colors.White);
        page.DefaultTextStyle(style => style.FontFamily("Times New Roman").FontSize(8).FontColor("#1E293B"));

        page.Footer().PaddingTop(6).BorderTop(0.5f).BorderColor(Rule).Row(row =>
        {
            row.RelativeItem().Text($"Yaradılma tarixi: {DateTimeOffset.UtcNow:dd.MM.yyyy HH:mm} UTC")
                .FontSize(7).FontColor(Muted);
            row.ConstantItem(90).AlignRight().DefaultTextStyle(style => style.FontSize(7).FontColor(Muted))
                .Text(text =>
                {
                    text.CurrentPageNumber();
                    text.Span(" / ");
                    text.TotalPages();
                });
        });

        page.Content().Column(column =>
        {
            column.Spacing(8);
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(header =>
                {
                    header.Item().Text(report.CompanyName).FontSize(9).Bold().FontColor(Accent);
                    header.Item().PaddingTop(3).Text("ƏMƏKHAQQI ÜMUMİ HESABATI")
                        .FontSize(16).Bold().FontColor(Navy);
                });

                row.ConstantItem(180).AlignRight().AlignMiddle().Column(meta =>
                {
                    meta.Item().AlignRight().Text("HESABLANMA DÖVRÜ").FontSize(7).Bold().FontColor(Muted);
                    meta.Item().PaddingTop(3).AlignRight()
                        .Text($"{AzMonths[report.Month - 1]} {report.Year}").FontSize(11).Bold().FontColor(Navy);
                    meta.Item().PaddingTop(2).AlignRight().Text($"Status: {RunStatus(report.Status)}")
                        .FontSize(8).FontColor(Muted);
                });
            });

            column.Item().PaddingTop(4).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(24);
                    columns.RelativeColumn(2.4f);
                    columns.RelativeColumn(1.05f);
                    columns.RelativeColumn(0.85f);
                    columns.RelativeColumn(0.9f);
                    columns.RelativeColumn(1.05f);
                    columns.RelativeColumn(1.05f);
                    columns.RelativeColumn(1.15f);
                    columns.RelativeColumn(1.0f);
                    columns.RelativeColumn(1.15f);
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderCell).Text("№");
                    header.Cell().Element(HeaderCell).Text("İşçi");
                    header.Cell().Element(HeaderCell).AlignRight().Text("Əsas maaş");
                    header.Cell().Element(HeaderCell).AlignRight().Text("Bonus");
                    header.Cell().Element(HeaderCell).AlignRight().Text("Düzəliş");
                    header.Cell().Element(HeaderCell).AlignRight().Text("Gross");
                    header.Cell().Element(HeaderCell).AlignRight().Text("Xalis");
                    header.Cell().Element(HeaderCell).AlignRight().Text("İşəgötürən xərci");
                    header.Cell().Element(HeaderCell).Text("Ödəniş üsulu");
                    header.Cell().Element(HeaderCell).Text("Ödəniş statusu");
                });

                var index = 0;
                foreach (var employee in report.Employees)
                {
                    index++;
                    ReportCell(table, index.ToString(Az));
                    ReportCell(table, employee.EmployeeFullName);
                    ReportMoneyCell(table, employee.BaseSalary);
                    ReportMoneyCell(table, employee.BonusAmount);
                    ReportMoneyCell(table, employee.AdjustmentAmount);
                    ReportMoneyCell(table, employee.GrossSalary);
                    ReportMoneyCell(table, employee.FinalNetSalary);
                    ReportMoneyCell(table, employee.TotalEmployerCost);
                    ReportCell(table, PaymentMethodName(employee.PaymentMethod));
                    ReportCell(table, employee.PaidAt is { } paidAt
                        ? $"Ödənilib\n{paidAt:dd.MM.yyyy}"
                        : "Gözləyir");
                }

                table.Cell().Element(TotalCell).Text(string.Empty);
                table.Cell().Element(TotalCell).Text($"CƏMİ · {report.Employees.Count} işçi");
                ReportTotalMoneyCell(table, report.Employees.Sum(e => e.BaseSalary));
                ReportTotalMoneyCell(table, report.Employees.Sum(e => e.BonusAmount));
                ReportTotalMoneyCell(table, report.Employees.Sum(e => e.AdjustmentAmount));
                ReportTotalMoneyCell(table, report.Employees.Sum(e => e.GrossSalary));
                ReportTotalMoneyCell(table, report.Employees.Sum(e => e.FinalNetSalary));
                ReportTotalMoneyCell(table, report.Employees.Sum(e => e.TotalEmployerCost));
                table.Cell().Element(TotalCell).Text(string.Empty);
                table.Cell().Element(TotalCell).Text(string.Empty);
            });
        });
    }

    private static void ReportCell(TableDescriptor table, string value) =>
        table.Cell().Element(DataCell).Text(value);

    private static void ReportMoneyCell(TableDescriptor table, decimal value) =>
        table.Cell().Element(DataCell).AlignRight().Text(Money(value));

    private static void ReportTotalMoneyCell(TableDescriptor table, decimal value) =>
        table.Cell().Element(TotalCell).AlignRight().Text(Money(value));

    private static IContainer HeaderCell(IContainer container) => container
        .Background(Navy).PaddingVertical(7).PaddingHorizontal(5)
        .DefaultTextStyle(style => style.FontSize(7).Bold().FontColor(Colors.White));

    private static IContainer DataCell(IContainer container) => container
        .BorderBottom(0.5f).BorderColor(Rule).PaddingVertical(6).PaddingHorizontal(5);

    private static IContainer TotalCell(IContainer container) => container
        .Background(SoftBackground).BorderBottom(0.7f).BorderColor(Rule).PaddingVertical(7).PaddingHorizontal(5)
        .DefaultTextStyle(style => style.FontSize(7.5f).Bold().FontColor(Navy));

    private static string Money(decimal amount) => $"{amount.ToString("N2", Az)} ₼";

    private static string PaymentMethodName(string? method) => method switch
    {
        null => "—",
        "Cash" => "Nağd",
        "BankTransfer" => "Bank köçürməsi",
        "Card" => "Kart",
        _ => method
    };

    private static string RunStatus(string status) => status switch
    {
        "Draft" => "Qaralama",
        "Calculated" => "Hesablanıb",
        "Approved" => "Təsdiqlənib",
        "Paid" => "Ödənilib",
        _ => status
    };

    private static CultureInfo CreateAzCulture()
    {
        var culture = (CultureInfo)CultureInfo.GetCultureInfo("az-Latn-AZ").Clone();
        culture.NumberFormat.NumberDecimalSeparator = ",";
        culture.NumberFormat.NumberGroupSeparator = ".";
        return culture;
    }

    private static void ComposePayslipPage(PageDescriptor page, PayslipExportData data)
    {
        page.Size(PageSizes.A4);
        page.Margin(36);
        page.PageColor(Colors.White);
        page.DefaultTextStyle(x => x.FontSize(11f).FontFamily("Times New Roman").FontColor(Colors.Grey.Darken4));

        page.Content().Column(column =>
        {
            column.Spacing(6);

            // ---- Header ----
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(head =>
                {
                    head.Item().Text(data.CompanyName.ToUpper(new CultureInfo("az-AZ")))
                        .FontSize(9f).FontColor(AccentColor).LetterSpacing(0.05f);
                    head.Item().Text("Əmək haqqı qəbzi").FontSize(18).Bold().FontColor(Colors.Grey.Darken4);
                });

                row.ConstantItem(140).AlignRight().Text($"{AzMonths[data.Month - 1]} {data.Year}")
                    .FontSize(11).Bold().FontColor(Colors.Grey.Darken2);
            });

            column.Item().LineHorizontal(1f).LineColor(AccentColor);

            // ---- Employee ----
            column.Item().PaddingTop(4).Text(text =>
            {
                text.Span("İşçi: ").FontColor(Colors.Grey.Darken2);
                text.Span(data.EmployeeFullName).Bold().FontSize(13);
            });

            column.Item().PaddingTop(8);

            // ---- Earnings ----
            Line(column, "Əsas əmək haqqı", data.BaseSalary);
            if (data.BonusAmount != 0)
            {
                Line(column, "Bonus", data.BonusAmount);
            }

            column.Item().PaddingTop(2).LineHorizontal(0.75f).LineColor(RuleColor);
            Line(column, "Ümumi (gross)", data.GrossSalary, bold: true);

            column.Item().PaddingTop(6).LineHorizontal(1.5f).LineColor(AccentColor);
            column.Item().PaddingTop(4).Row(row =>
            {
                row.RelativeItem().Text("Ələ keçən məbləğ".ToUpper(new CultureInfo("az-AZ")))
                    .Bold().FontSize(13).FontColor(Colors.Grey.Darken4);
                row.ConstantItem(120).AlignRight().Text($"{data.FinalNetSalary:N2} AZN")
                    .Bold().FontSize(15).FontColor(AccentColor);
            });

            column.Item().PaddingTop(24).AlignCenter().Text(
                "Bu sənəd elektron qaydada yaradılıb və imza tələb etmir.")
                .FontSize(8.5f).FontColor(Colors.Grey.Darken1).Italic();
        });
    }

    private static void Line(ColumnDescriptor column, string label, decimal amount, bool bold = false)
    {
        column.Item().Row(row =>
        {
            var labelText = row.RelativeItem().Text(label).FontColor(Colors.Grey.Darken2);
            var valueText = row.ConstantItem(100).AlignRight().Text($"{amount:N2}");

            if (bold)
            {
                labelText.Bold().FontColor(Colors.Grey.Darken4);
                valueText.Bold();
            }
        });
    }
}
