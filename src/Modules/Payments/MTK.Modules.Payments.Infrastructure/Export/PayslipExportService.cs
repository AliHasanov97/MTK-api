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
    private static readonly string AccentColor = Colors.Blue.Darken2;
    private static readonly string RuleColor = Colors.Grey.Lighten1;

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

    public MemoryStream ExportRunToPdf(IReadOnlyCollection<PayslipExportData> payslips)
    {
        var stream = new MemoryStream();

        Document.Create(container =>
        {
            foreach (var payslip in payslips)
            {
                container.Page(page => ComposePayslipPage(page, payslip));
            }
        }).GeneratePdf(stream);

        return stream;
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
