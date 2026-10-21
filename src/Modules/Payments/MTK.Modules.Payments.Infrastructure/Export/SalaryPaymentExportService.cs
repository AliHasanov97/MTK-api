using System.Globalization;
using MTK.Modules.Payments.Application.Abstractions.Services.Export;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MTK.Modules.Payments.Infrastructure.Export;

/// <summary>Maaş ödənişi qəbzi PDF ixracı.</summary>
internal sealed class SalaryPaymentExportService : ISalaryPaymentExportService
{
    private static readonly string AccentColor = Colors.Blue.Darken2;
    private static readonly string RuleColor = Colors.Grey.Lighten1;

    private static readonly string[] AzMonths =
    [
        "Yanvar", "Fevral", "Mart", "Aprel", "May", "İyun",
        "İyul", "Avqust", "Sentyabr", "Oktyabr", "Noyabr", "Dekabr",
    ];

    public MemoryStream ExportToPdf(SalaryPaymentExportData data)
    {
        var stream = new MemoryStream();
        Document.Create(container => container.Page(page => ComposePage(page, data)))
            .GeneratePdf(stream);
        return stream;
    }

    private static void ComposePage(PageDescriptor page, SalaryPaymentExportData data)
    {
        page.Size(PageSizes.A4);
        page.Margin(36);
        page.PageColor(Colors.White);
        page.DefaultTextStyle(style => style.FontSize(11).FontFamily("Times New Roman").FontColor(Colors.Grey.Darken4));

        page.Content().Column(column =>
        {
            column.Spacing(6);
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(header =>
                {
                    header.Item().Text(data.CompanyName.ToUpper(new CultureInfo("az-AZ")))
                        .FontSize(9).FontColor(AccentColor).LetterSpacing(0.05f);
                    header.Item().Text("Əmək haqqı qəbzi").FontSize(18).Bold();
                });
                row.ConstantItem(140).AlignRight()
                    .Text($"{AzMonths[data.Month - 1]} {data.Year}").FontSize(11).Bold();
            });

            column.Item().LineHorizontal(1).LineColor(AccentColor);
            column.Item().PaddingTop(4).Text(text =>
            {
                text.Span("İşçi: ").FontColor(Colors.Grey.Darken2);
                text.Span(data.EmployeeFullName).Bold().FontSize(13);
            });

            column.Item().PaddingTop(8);
            Line(column, "Əsas əmək haqqı", data.BaseSalary);

            foreach (var line in data.Lines)
                Line(column, line.Description, line.Amount);

            column.Item().PaddingTop(2).LineHorizontal(0.75f).LineColor(RuleColor);
            Line(column, "Ümumi (gross)", data.GrossSalary, bold: true);

            column.Item().PaddingTop(6).LineHorizontal(1.5f).LineColor(AccentColor);
            column.Item().PaddingTop(4).Row(row =>
            {
                row.RelativeItem().Text("ƏLƏ KEÇƏN MƏBLƏĞ")
                    .Bold().FontSize(13).FontColor(Colors.Grey.Darken4);
                row.ConstantItem(120).AlignRight().Text($"{data.GrossSalary:N2} AZN")
                    .Bold().FontSize(15).FontColor(AccentColor);
            });

            column.Item().PaddingTop(12).Text(text =>
            {
                text.Span("Ödəniş metodu: ").FontColor(Colors.Grey.Darken2);
                text.Span(GetPaymentMethodName(data.PaymentMethod));
            });
            column.Item().Text(text =>
            {
                text.Span("Ödəniş tarixi: ").FontColor(Colors.Grey.Darken2);
                text.Span(data.PaymentDate.ToString("dd.MM.yyyy"));
            });

            if (!string.IsNullOrWhiteSpace(data.Notes))
                column.Item().PaddingTop(4).Text(text =>
                {
                    text.Span("Qeyd: ").FontColor(Colors.Grey.Darken2);
                    text.Span(data.Notes);
                });

            column.Item().PaddingTop(24).AlignCenter()
                .Text("Bu sənəd elektron qaydada yaradılıb və imza tələb etmir.")
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

    private static string GetPaymentMethodName(string method) => method switch
    {
        "Cash" => "Nağd",
        "BankTransfer" => "Bank köçürməsi",
        "Card" => "Kart",
        _ => method
    };
}
