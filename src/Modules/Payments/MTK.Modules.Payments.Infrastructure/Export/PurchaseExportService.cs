using System.Globalization;
using MTK.Modules.Payments.Application.Abstractions.Services.Export;
using MTK.Modules.Payments.Application.Purchases.Queries.GetPurchaseById;
using MTK.Modules.Payments.Domain.Purchases;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MTK.Modules.Payments.Infrastructure.Export;

/// <summary>Satınalma sənədini PDF formatında yaradır.</summary>
internal sealed class PurchaseExportService : IPurchaseExportService
{
    private const string Navy = "#0F2A43";
    private const string Accent = "#1D4E89";
    private const string Rule = "#D7E0EA";
    private const string Muted = "#5B6B7C";
    private const string SoftBackground = "#F7F9FC";

    private static readonly CultureInfo Az = CreateAzCulture();

    public MemoryStream ExportToPdf(PurchaseResponse purchase)
    {
        var stream = new MemoryStream();
        Document.Create(container => container.Page(page => ComposePage(page, purchase)))
            .GeneratePdf(stream);
        stream.Position = 0;
        return stream;
    }

    private static void ComposePage(PageDescriptor page, PurchaseResponse purchase)
    {
        page.Size(PageSizes.A4);
        page.Margin(32);
        page.PageColor(Colors.White);
        page.DefaultTextStyle(style => style.FontFamily("Times New Roman").FontSize(9).FontColor("#1E293B"));

        page.Footer().PaddingTop(7).BorderTop(0.5f).BorderColor(Rule).Row(row =>
        {
            row.RelativeItem().Text($"Yaradılma tarixi: {DateTimeOffset.UtcNow:dd.MM.yyyy HH:mm} UTC")
                .FontSize(7).FontColor(Muted);
            row.ConstantItem(80).AlignRight().DefaultTextStyle(style => style.FontSize(7).FontColor(Muted))
                .Text(text =>
                {
                    text.CurrentPageNumber();
                    text.Span(" / ");
                    text.TotalPages();
                });
        });

        page.Content().Column(column =>
        {
            column.Spacing(9);
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(header =>
                {
                    header.Item().Text("SATINALMA SƏNƏDİ").FontSize(17).Bold().FontColor(Navy);
                    header.Item().PaddingTop(3).Text($"Sənəd № {purchase.InvoiceNumber ?? purchase.Id.ToString("N")[..8].ToUpperInvariant()}")
                        .FontSize(9).FontColor(Muted);
                });
                row.ConstantItem(130).AlignRight().AlignMiddle()
                    .Text(StatusLabel(purchase.Status)).FontSize(10).Bold().FontColor(Accent);
            });

            column.Item().LineHorizontal(1).LineColor(Accent);
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                });

                MetaCell(table, "TƏDARÜKÇÜ", purchase.VendorName ?? "—");
                MetaCell(table, "SATINALMA TARİXİ", purchase.PurchaseDate.ToString("dd.MM.yyyy"));
                MetaCell(table, "QƏBUL TARİXİ", purchase.ReceivedOnUtc?.ToString("dd.MM.yyyy") ?? "—");
                MetaCell(table, "STATUS", StatusLabel(purchase.Status));
            });

            column.Item().PaddingTop(3).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(25);
                    columns.RelativeColumn(1.1f);
                    columns.RelativeColumn(3.2f);
                    columns.RelativeColumn(0.9f);
                    columns.RelativeColumn(1.1f);
                    columns.RelativeColumn(1.2f);
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderCell).Text("№");
                    header.Cell().Element(HeaderCell).Text("Kod");
                    header.Cell().Element(HeaderCell).Text("Mal / material");
                    header.Cell().Element(HeaderCell).AlignRight().Text("Miqdar");
                    header.Cell().Element(HeaderCell).AlignRight().Text("Vahid qiymət");
                    header.Cell().Element(HeaderCell).AlignRight().Text("Məbləğ");
                });

                var index = 0;
                foreach (var line in purchase.Lines)
                {
                    index++;
                    table.Cell().Element(DataCell).Text(index.ToString(Az));
                    table.Cell().Element(DataCell).Text(line.NomenclatureCode ?? "—");
                    table.Cell().Element(DataCell).Text(line.NomenclatureName ?? "—");
                    table.Cell().Element(DataCell).AlignRight().Text(Quantity(line.Quantity));
                    table.Cell().Element(DataCell).AlignRight().Text(Money(line.UnitPrice));
                    table.Cell().Element(DataCell).AlignRight().Text(Money(line.LineTotal));
                }

                table.Cell().Element(TotalCell).Text(string.Empty);
                table.Cell().Element(TotalCell).Text(string.Empty);
                table.Cell().Element(TotalCell).Text("CƏMİ");
                table.Cell().Element(TotalCell).Text(string.Empty);
                table.Cell().Element(TotalCell).Text(string.Empty);
                table.Cell().Element(TotalCell).AlignRight().Text(Money(purchase.TotalAmount));
            });

            if (!string.IsNullOrWhiteSpace(purchase.Note))
            {
                column.Item().PaddingTop(3).Background(SoftBackground).BorderLeft(2).BorderColor(Accent)
                    .Padding(9).Row(row =>
                    {
                        row.ConstantItem(50).Text("QEYD").FontSize(7).Bold().FontColor(Muted);
                        row.RelativeItem().Text(purchase.Note).FontSize(9);
                    });
            }
        });
    }

    private static void MetaCell(TableDescriptor table, string label, string value)
    {
        table.Cell().BorderBottom(0.5f).BorderColor(Rule).PaddingVertical(6).PaddingHorizontal(8).Column(cell =>
        {
            cell.Item().Text(label).FontSize(7).Bold().FontColor(Muted);
            cell.Item().PaddingTop(2).Text(value).FontSize(9).Bold().FontColor(Navy);
        });
    }

    private static IContainer HeaderCell(IContainer container) => container
        .Background(Navy).PaddingVertical(7).PaddingHorizontal(6)
        .DefaultTextStyle(style => style.FontSize(7.5f).Bold().FontColor(Colors.White));

    private static IContainer DataCell(IContainer container) => container
        .BorderBottom(0.5f).BorderColor(Rule).PaddingVertical(6).PaddingHorizontal(6);

    private static IContainer TotalCell(IContainer container) => container
        .Background(SoftBackground).BorderBottom(0.7f).BorderColor(Rule).PaddingVertical(7).PaddingHorizontal(6)
        .DefaultTextStyle(style => style.FontSize(8).Bold().FontColor(Navy));

    private static string StatusLabel(PurchaseStatus status) => status switch
    {
        PurchaseStatus.Draft => "Qaralama",
        PurchaseStatus.Received => "Qəbul edilib",
        PurchaseStatus.Cancelled => "Ləğv edilib",
        _ => status.ToString()
    };

    private static string Money(decimal amount) => $"{amount.ToString("N2", Az)} ₼";

    private static string Quantity(decimal amount) => amount.ToString("N2", Az);

    private static CultureInfo CreateAzCulture()
    {
        var culture = (CultureInfo)CultureInfo.GetCultureInfo("az-Latn-AZ").Clone();
        culture.NumberFormat.NumberDecimalSeparator = ",";
        culture.NumberFormat.NumberGroupSeparator = ".";
        return culture;
    }
}
