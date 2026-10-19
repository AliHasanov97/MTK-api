using System.Globalization;
using MTK.Modules.Payments.Application.Abstractions.Services.Export;
using MTK.Modules.Payments.Application.Charges.Queries.GetAnnualPaymentReport;
using MTK.Modules.Payments.Domain.Charges;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MTK.Modules.Payments.Infrastructure.Export;

/// <summary>
/// İllik ödəniş qrafikinin PDF versiyası — Excel faylındakı eyni sütunlar və
/// işarələr (+ / ± / −), A4 landscape. Uzun siyahılar üçün cədvəl başlığı hər
/// səhifədə təkrarlanır.
/// </summary>
internal sealed class AnnualPaymentReportPdfExportService : IAnnualPaymentReportPdfExportService
{
    private const string Navy = "#0F2A43";
    private const string Accent = "#1D4E89";
    private const string Rule = "#D7E0EA";
    private const string Muted = "#5B6B7C";
    private const string TextColor = "#1E293B";

    private static readonly CultureInfo Az = CreateAzCulture();

    private static readonly string[] MonthNames =
    [
        "Yan", "Fev", "Mar", "Apr", "May", "İyn",
        "İyl", "Avq", "Sen", "Okt", "Noy", "Dek",
    ];

    public MemoryStream ExportToPdf(
        AnnualPaymentReportResponse report,
        IReadOnlyDictionary<Guid, PropertyExportLabel> labels,
        string companyName)
    {
        var stream = new MemoryStream();

        Document.Create(container => container.Page(page =>
        {
            page.Size(PageSizes.A4.Landscape());
            page.Margin(24);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontFamily("Times New Roman").FontSize(7.5f).FontColor(TextColor));

            page.Footer().PaddingTop(5).Row(row =>
            {
                row.RelativeItem().Text("Bu sənəd elektron qaydada yaradılıb.")
                    .FontSize(6.8f).FontColor(Muted).Italic();
                row.ConstantItem(80).AlignRight()
                    .DefaultTextStyle(style => style.FontSize(6.8f).FontColor(Muted))
                    .Text(text =>
                    {
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
            });

            page.Content().Column(column =>
            {
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(head =>
                    {
                        head.Item().Text(companyName.ToUpper(Az)).FontSize(8).FontColor(Accent).LetterSpacing(0.08f);
                        head.Item().Text("İLLİK ÖDƏNİŞ QRAFİKİ").FontSize(17).Bold().FontColor(Navy);
                        head.Item().Text($"{report.Year}-ci il üzrə mənzil və qarajların aylıq ödəniş statusu")
                            .FontSize(7.5f).FontColor(Muted);
                    });

                    row.ConstantItem(140).AlignRight().Column(meta =>
                    {
                        meta.Item().AlignRight().Text($"{report.Year}").FontSize(16).Bold().FontColor(Navy);
                        meta.Item().AlignRight().Text($"Cəmi əmlak: {report.Properties.Count}")
                            .FontSize(7.5f).FontColor(Muted);
                    });
                });

                column.Item().PaddingTop(5).LineHorizontal(1.2f).LineColor(Navy);

                column.Item().PaddingTop(6).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(22);  // №
                        columns.ConstantColumn(36);  // Növ
                        columns.ConstantColumn(42);  // Nömrə
                        columns.ConstantColumn(78);  // Bina
                        columns.ConstantColumn(102); // Sahibi
                        for (var i = 0; i < 12; i++)
                        {
                            columns.ConstantColumn(24);
                        }
                        columns.ConstantColumn(46);  // Hesablanıb
                        columns.ConstantColumn(46);  // Ödənilib
                        columns.ConstantColumn(46);  // Cari borc
                    });

                    table.Header(header =>
                    {
                        header.Cell().RowSpan(2).PdfHeaderCell().Text("№");
                        header.Cell().RowSpan(2).PdfHeaderCell().Text("Növ");
                        header.Cell().RowSpan(2).PdfHeaderCell().Text("Nömrə");
                        header.Cell().RowSpan(2).PdfHeaderCell().Text("Bina");
                        header.Cell().RowSpan(2).PdfHeaderCell().Text("Sahibi");

                        for (var i = 0; i < 12; i++)
                        {
                            header.Cell().PdfHeaderCell().AlignCenter().Text(MonthNames[i]);
                        }

                        header.Cell().RowSpan(2).PdfHeaderCell().AlignRight().Text("Hesablanıb");
                        header.Cell().RowSpan(2).PdfHeaderCell().AlignRight().Text("Ödənilib");
                        header.Cell().RowSpan(2).PdfHeaderCell().AlignRight().Text("Cari borc");
                    });

                    var serial = 1;
                    decimal totalAmount = 0, totalPaid = 0, totalDebt = 0;

                    var ordered = report.Properties
                        .Select(p => (Property: p, Label: labels.GetValueOrDefault(p.PropertyId)))
                        .OrderBy(x => x.Property.PropertyType == PropertyType.Garage ? 1 : 0)
                        .ThenBy(x => x.Label?.Building, StringComparer.CurrentCultureIgnoreCase)
                        .ThenBy(x => x.Label?.Number, StringComparer.CurrentCultureIgnoreCase)
                        .ToList();

                    foreach (var (property, label) in ordered)
                    {
                        table.Cell().PdfDataCell().Text(serial.ToString());
                        table.Cell().PdfDataCell()
                            .Text(property.PropertyType == PropertyType.Garage ? "Qaraj" : "Mənzil");
                        table.Cell().PdfDataCell().Text(label?.Number ?? "—");
                        table.Cell().PdfDataCell().Text(label?.Building ?? "—");
                        table.Cell().PdfDataCell().Text(label?.Owner ?? "—");

                        foreach (var month in property.Months)
                        {
                            var color = month.Status switch
                            {
                                ChargeStatus.Paid => "#DCFCE7",
                                ChargeStatus.PartiallyPaid => "#FEF3C7",
                                ChargeStatus.Unpaid => "#FEE2E2",
                                _ => "#FFFFFF",
                            };

                            var symbol = month.Status switch
                            {
                                ChargeStatus.Paid => "+",
                                ChargeStatus.PartiallyPaid => "±",
                                ChargeStatus.Unpaid => "−",
                                _ => "—",
                            };

                        StatusCell(table.Cell(), color).AlignCenter().Text(symbol).Bold();
                        }

                        var amount = property.Months.Sum(m => m.Amount);
                        var paid = property.Months.Sum(m => m.PaidAmount);
                        totalAmount += amount;
                        totalPaid += paid;
                        totalDebt += property.CurrentDebt;

                        table.Cell().PdfDataCell().AlignRight().Text(Money(amount));
                        table.Cell().PdfDataCell().AlignRight().Text(Money(paid));
                        table.Cell().PdfDataCell().AlignRight().Text(Money(property.CurrentDebt)).Bold();
                        serial++;
                    }

                    table.Cell().ColumnSpan(5).PdfTotalCell().AlignRight().Text("YEKUN");
                    for (var i = 0; i < 12; i++)
                    {
                        table.Cell().PdfTotalCell().Text(string.Empty);
                    }
                    table.Cell().PdfTotalCell().AlignRight().Text(Money(totalAmount));
                    table.Cell().PdfTotalCell().AlignRight().Text(Money(totalPaid));
                    table.Cell().PdfTotalCell().AlignRight().Text(Money(totalDebt));
                });

                column.Item().PaddingTop(8).Text("ŞƏRTLİ İŞARƏLƏR").FontSize(7.5f).Bold().FontColor(Navy);
                column.Item().PaddingTop(3).Row(row =>
                {
                    row.Spacing(14);
                    Legend(row, "+", "Tam ödənilib", "#DCFCE7");
                    Legend(row, "±", "Qismən ödənilib", "#FEF3C7");
                    Legend(row, "−", "Ödənilməyib (borc)", "#FEE2E2");
                    Legend(row, "—", "Həmin ay hesablama yoxdur", "#F1F5F9");
                });
            });
        })).GeneratePdf(stream);

        stream.Position = 0;
        return stream;
    }

    private static void Legend(RowDescriptor row, string symbol, string label, string background)
    {
        row.AutoItem().Row(item =>
        {
            item.ConstantItem(18).Background(background).Border(0.4f).BorderColor(Rule)
                .AlignCenter().AlignMiddle().Text(symbol).Bold().FontSize(7.5f);
            item.ConstantItem(5);
            item.AutoItem().Text(label).FontSize(7.2f).FontColor(Muted);
        });
    }

    private static IContainer StatusCell(IContainer container, string background) =>
        container
            .Background(background)
            .BorderBottom(0.4f).BorderColor(Rule)
            .PaddingVertical(2.5f).PaddingHorizontal(2)
            .DefaultTextStyle(style => style.FontSize(7.5f).FontColor(Navy));

    private static string Money(decimal amount) => $"{amount.ToString("N2", Az)} ₼";

    private static CultureInfo CreateAzCulture()
    {
        try
        {
            return CultureInfo.GetCultureInfo("az-AZ");
        }
        catch (CultureNotFoundException)
        {
            return CultureInfo.InvariantCulture;
        }
    }
}

internal static class AnnualPaymentReportPdfStyles
{
    public static IContainer PdfHeaderCell(this IContainer container) =>
        container
            .Background("#0F2A43")
            .Border(0.4f).BorderColor("#FFFFFF")
            .PaddingVertical(3).PaddingHorizontal(3)
            .DefaultTextStyle(style => style.FontSize(6.8f).Bold().FontColor(Colors.White));

    public static IContainer PdfDataCell(this IContainer container) =>
        container
            .BorderBottom(0.4f).BorderColor("#D7E0EA")
            .PaddingVertical(2.5f).PaddingHorizontal(3)
            .DefaultTextStyle(style => style.FontSize(6.9f).FontColor("#1E293B"));

    public static IContainer PdfTotalCell(this IContainer container) =>
        container
            .Background("#EDF2F7")
            .BorderTop(0.8f).BorderColor("#0F2A43")
            .PaddingVertical(3).PaddingHorizontal(3)
            .DefaultTextStyle(style => style.FontSize(7f).Bold().FontColor("#0F2A43"));
}
