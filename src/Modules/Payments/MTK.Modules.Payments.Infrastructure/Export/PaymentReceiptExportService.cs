using System.Globalization;
using MTK.Modules.Payments.Application.Abstractions.Services.Export;
using MTK.Modules.Payments.Domain.Parties;
using MTK.Modules.Payments.Domain.Payments;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MTK.Modules.Payments.Infrastructure.Export;

/// <summary>Payment receipt styled to match the monthly report PDF.</summary>
internal sealed class PaymentReceiptExportService : IPaymentReceiptExportService
{
    private const string Navy = "#0F2A43";
    private const string Accent = "#1D4E89";
    private const string Rule = "#D7E0EA";
    private const string Muted = "#5B6B7C";
    private const string TextColor = "#1E293B";
    private const string SoftBackground = "#F7F9FC";
    private const string Green = "#067647";
    private const string GreenBackground = "#ECFDF3";

    private static readonly CultureInfo Az = CultureInfo.GetCultureInfo("az-Latn-AZ");
    private static readonly string[] AzMonths =
    [
        "Yanvar", "Fevral", "Mart", "Aprel", "May", "İyun",
        "İyul", "Avqust", "Sentyabr", "Oktyabr", "Noyabr", "Dekabr",
    ];

    public MemoryStream ExportToPdf(PaymentReceiptData data)
    {
        var stream = new MemoryStream();

        Document.Create(document => document.Page(page =>
        {
            page.Size(PageSizes.A5);
            page.Margin(30);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(style => style
                .FontFamily("Times New Roman")
                .FontSize(9)
                .FontColor(TextColor));

            page.Footer().PaddingTop(8).Row(row =>
            {
                row.RelativeItem().Text("Bu qəbz elektron qaydada yaradılıb.")
                    .FontSize(7).FontColor(Muted).Italic();
                row.ConstantItem(70).AlignRight().DefaultTextStyle(style => style.FontSize(7).FontColor(Muted))
                    .Text(text =>
                    {
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
            });

            page.Content().Column(column =>
            {
                column.Spacing(7);
                ComposeHeader(column, data);
                ComposePaymentSummary(column, data);
                ComposeAllocation(column, data);
                ComposeNotes(column, data);
                ComposeSignature(column, data);
            });
        })).GeneratePdf(stream);

        stream.Position = 0;
        return stream;
    }

    private static void ComposeHeader(ColumnDescriptor column, PaymentReceiptData data)
    {
        column.Item().Row(row =>
        {
            row.RelativeItem().Column(company =>
            {
                company.Item().Text(data.CompanyName.ToUpper(Az))
                    .FontSize(8).Bold().FontColor(Accent).LetterSpacing(0.06f);
                company.Item().PaddingTop(2).Text("ÖDƏNİŞ QƏBZİ")
                    .FontSize(21).Bold().FontColor(Navy);
                company.Item().Text(data.PartyType == PartyType.Owner
                        ? "Sakin tərəfindən edilmiş ödəniş"
                        : "Tədarükçüyə edilmiş ödəniş")
                    .FontSize(8).FontColor(Muted);
            });

            row.ConstantItem(110).AlignRight().Column(meta =>
            {
                meta.Item().AlignRight().Text($"Qəbz № {data.ReceiptNumber:D8}")
                    .FontSize(8).FontColor(Muted);
                meta.Item().PaddingTop(3).AlignRight()
                    .Text(data.PaymentDate.ToString("dd.MM.yyyy", Az))
                    .FontSize(12).Bold().FontColor(Navy);
                meta.Item().AlignRight().Text(data.PaymentDate.ToString("HH:mm", Az))
                    .FontSize(8).FontColor(Muted);
            });
        });

        column.Item().PaddingTop(3).LineHorizontal(1.4f).LineColor(Navy);
    }

    private static void ComposePaymentSummary(ColumnDescriptor column, PaymentReceiptData data)
    {
        Section(column, data.PartyType == PartyType.Owner ? "ÖDƏNİŞ EDƏN" : "ÖDƏNİŞ ALAN");
        column.Item().Row(row =>
        {
            row.RelativeItem().Border(0.7f).BorderColor(Rule).Background(SoftBackground)
                .Padding(10).Column(party =>
                {
                    party.Item().Text(data.PayerName ?? "Məlumat göstərilməyib")
                        .FontSize(11).Bold().FontColor(Navy);
                    if (data.PartyType == PartyType.Owner
                        && !string.IsNullOrWhiteSpace(data.PayerEmail))
                    {
                        party.Item().PaddingTop(3).Text(data.PayerEmail)
                            .FontSize(8).FontColor(Muted);
                    }
                });

            row.ConstantItem(10);
            row.ConstantItem(120).Background("#EFF6FF").Padding(10).Column(amount =>
            {
                amount.Item().Text("ÖDƏNİŞ MƏBLƏĞİ")
                    .FontSize(7).Bold().FontColor(Accent).LetterSpacing(0.04f);
                amount.Item().PaddingTop(5).Text(Money(data.Amount))
                    .FontSize(16).Bold().FontColor(Navy);
                amount.Item().Text("AZN").FontSize(8).FontColor(Muted);
            });
        });

        column.Item().PaddingTop(1).Row(row =>
        {
            row.RelativeItem().Text(text =>
            {
                text.Span("Ödəniş üsulu: ").FontColor(Muted);
                text.Span(MethodLabel(data.PaymentMethod)).Bold();
            });
            if (data.Period is not null)
            {
                row.RelativeItem().AlignRight().Text($"Hesablaşma dövrü: {FormatPeriodLabel(data.Period)}")
                    .FontColor(Muted);
            }
        });
    }

    private static void ComposeAllocation(ColumnDescriptor column, PaymentReceiptData data)
    {
        Section(column, data.PartyType == PartyType.Owner ? "BORCA TƏTBİQ OLUNAN MƏBLƏĞ" : "ÖDƏNİŞİN TƏYİNATI");

        if (data.Lines.Count == 0)
        {
            var recipient = data.PartyType == PartyType.Owner ? "sakin" : "tədarükçü";
            column.Item().Border(0.7f).BorderColor(Rule).Background(SoftBackground).Padding(10)
                .Text($"Bu ödəniş hələ hesablanmış borc və ya xərc maddəsinə tətbiq edilməyib. " +
                      $"{Money(data.Amount)} AZN {recipient} hesabında avans kimi saxlanılır.")
                .FontSize(8.5f).LineHeight(1.3f);
            return;
        }

        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(3);
                columns.RelativeColumn(2);
                columns.ConstantColumn(70);
            });

            table.Header(header =>
            {
                header.Cell().Element(HeaderCell).Text(data.PartyType == PartyType.Owner ? "Borcun aid olduğu əmlak" : "Xərc / xidmət");
                header.Cell().Element(HeaderCell).Text("Dövr / açıqlama");
                header.Cell().Element(HeaderCell).AlignRight().Text("Məbləğ");
            });

            foreach (var line in data.Lines)
            {
                string item = line.PropertyLabel ?? line.Description;
                string detail = line.Period is not null
                    ? FormatPeriodLabel(line.Period) ?? line.Description
                    : line.Description;

                table.Cell().Element(BodyCell).Text(item);
                table.Cell().Element(BodyCell).Text(detail);
                table.Cell().Element(BodyCell).AlignRight().Text(Money(line.Amount));
            }

            table.Cell().ColumnSpan(2).Element(FooterCell).AlignRight().Text("Tətbiq olunan cəmi");
            table.Cell().Element(FooterCell).AlignRight().Text(Money(data.Lines.Sum(line => line.Amount)));
        });

        var appliedTotal = data.Lines.Sum(line => line.Amount);
        var remainder = Math.Max(0, data.Amount - appliedTotal);
        if (remainder > 0.004m)
        {
            var recipient = data.PartyType == PartyType.Owner ? "sakin" : "tədarükçü";
            column.Item().PaddingTop(4).Border(0.7f).BorderColor("#ABEFC6")
                .Background(GreenBackground).Padding(8).Text(
                    $"Tətbiq olunmamış qalıq: {Money(remainder)} AZN. Məbləğ {recipient} hesabında avans kimi saxlanılır.")
                .FontSize(8).FontColor(Green);
        }
    }

    private static void ComposeNotes(ColumnDescriptor column, PaymentReceiptData data)
    {
        if (string.IsNullOrWhiteSpace(data.Notes))
            return;

        Section(column, "ƏLAVƏ QEYD");
        column.Item().Text(data.Notes).FontSize(8.5f).FontColor(TextColor).LineHeight(1.25f);
    }

    private static void ComposeSignature(ColumnDescriptor column, PaymentReceiptData data)
    {
        column.Item().PaddingTop(8).LineHorizontal(0.7f).LineColor(Rule);
        column.Item().PaddingTop(7).Row(row =>
        {
            row.RelativeItem().Column(info =>
            {
                info.Item().Text("Qəbzi rəsmiləşdirən")
                    .FontSize(7).FontColor(Muted);
                info.Item().PaddingTop(3).Text(data.IssuerName ?? "Məlumat göstərilməyib")
                    .FontSize(9).Bold().FontColor(Navy);
                if (!string.IsNullOrWhiteSpace(data.IssuerTitle))
                    info.Item().Text(data.IssuerTitle).FontSize(8).FontColor(Muted);
            });

            row.ConstantItem(105).AlignRight().Column(company =>
            {
                company.Item().AlignRight().Text(data.CompanyName)
                    .FontSize(8).Bold().FontColor(Navy);
                company.Item().PaddingTop(13).Width(100).AlignRight()
                    .LineHorizontal(0.6f).LineColor(Rule);
                company.Item().AlignRight().Text("İmza / möhür")
                    .FontSize(7).FontColor(Muted);
            });
        });
    }

    private static void Section(ColumnDescriptor column, string title)
    {
        column.Item().PaddingTop(3).Text(title).FontSize(8).Bold().FontColor(Navy)
            .LetterSpacing(0.04f);
        column.Item().PaddingTop(2).LineHorizontal(0.7f).LineColor(Rule);
    }

    private static IContainer HeaderCell(IContainer container) => container
        .Background(Navy).PaddingVertical(6).PaddingHorizontal(6)
        .DefaultTextStyle(style => style.FontSize(7).Bold().FontColor(Colors.White));

    private static IContainer BodyCell(IContainer container) => container
        .BorderBottom(0.5f).BorderColor(Rule).PaddingVertical(6).PaddingHorizontal(6)
        .DefaultTextStyle(style => style.FontSize(7.5f).FontColor(TextColor));

    private static IContainer FooterCell(IContainer container) => container
        .Background(SoftBackground).PaddingVertical(7).PaddingHorizontal(6)
        .DefaultTextStyle(style => style.FontSize(8).Bold().FontColor(Navy));

    private static string Money(decimal amount) => $"{amount.ToString("N2", Az)} AZN";

    private static string? FormatPeriodLabel(string? period)
    {
        if (period is not { Length: 7 }
            || !int.TryParse(period.AsSpan(0, 4), out var year)
            || !int.TryParse(period.AsSpan(5, 2), out var month)
            || month is < 1 or > 12)
            return null;

        return $"{AzMonths[month - 1]} {year}";
    }

    private static string MethodLabel(PaymentMethod method) => method switch
    {
        PaymentMethod.Cash => "Nağd",
        PaymentMethod.BankTransfer => "Bank köçürməsi",
        PaymentMethod.Card => "Kartla",
        _ => method.ToString(),
    };
}
