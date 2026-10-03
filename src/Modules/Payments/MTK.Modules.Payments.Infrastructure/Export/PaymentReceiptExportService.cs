using MTK.Modules.Payments.Application.Abstractions.Services.Export;
using MTK.Modules.Payments.Domain.Parties;
using MTK.Modules.Payments.Domain.Payments;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MTK.Modules.Payments.Infrastructure.Export;

internal sealed class PaymentReceiptExportService : IPaymentReceiptExportService
{
    private static readonly Dictionary<PaymentMethod, string> MethodLabels = new()
    {
        [PaymentMethod.Cash] = "Nağd",
        [PaymentMethod.BankTransfer] = "Bank köçürməsi",
        [PaymentMethod.Card] = "Kart",
    };

    public MemoryStream ExportToPdf(PaymentReceiptData data)
    {
        var stream = new MemoryStream();

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A5);
                page.Margin(30);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Content().Column(column =>
                {
                    column.Spacing(12);

                    column.Item().Element(c => ComposeHeader(c, data));
                    column.Item().Element(c => ComposeParties(c, data));
                    column.Item().Element(c => ComposeLines(c, data));
                    column.Item().Element(c => ComposeNote(c, data));
                });
            });
        }).GeneratePdf(stream);

        stream.Position = 0;
        return stream;
    }

    private static void ComposeHeader(IContainer container, PaymentReceiptData data)
    {
        container.Column(column =>
        {
            column.Spacing(4);

            column.Item().AlignCenter().Text("ÖDƏNİŞ QƏBZİ").FontSize(14).Bold();
            column.Item().AlignCenter().Text($"№ {data.PaymentId.ToString()[..8].ToUpperInvariant()}").FontSize(9);
            column.Item().PaddingTop(6).BorderBottom(1);
        });
    }

    private static void ComposeParties(IContainer container, PaymentReceiptData data)
    {
        container.Column(column =>
        {
            column.Spacing(4);

            column.Item().Text(text =>
            {
                text.Span(data.PartyType == PartyType.Vendor ? "Tədarükçü:  " : "Sakin:  ").FontSize(10);
                text.Span(data.PayerName ?? "—").FontSize(10).Bold();
            });

            if (data.PropertyLabel is not null)
            {
                column.Item().Text(text =>
                {
                    text.Span("Əmlak:  ").FontSize(10);
                    text.Span(data.PropertyLabel).FontSize(10).Bold();
                });
            }

            column.Item().Text(text =>
            {
                text.Span("Tarix:  ").FontSize(10);
                text.Span(data.PaymentDate.ToString("dd.MM.yyyy HH:mm")).FontSize(10);
            });

            column.Item().Text(text =>
            {
                text.Span("Ödəniş üsulu:  ").FontSize(10);
                text.Span(MethodLabels.GetValueOrDefault(data.PaymentMethod, data.PaymentMethod.ToString())).FontSize(10);
            });
        });
    }

    private static void ComposeLines(IContainer container, PaymentReceiptData data)
    {
        container.Column(column =>
        {
            column.Spacing(6);
            column.Item().PaddingTop(4);

            if (data.Lines.Count > 0)
            {
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3);
                        columns.ConstantColumn(80);
                    });

                    table.Header(header =>
                    {
                        header.Cell().BorderBottom(1).Padding(3).Text("Təyinat").FontSize(9).Bold();
                        header.Cell().BorderBottom(1).Padding(3).AlignRight().Text("Məbləğ (₼)").FontSize(9).Bold();
                    });

                    foreach (var line in data.Lines)
                    {
                        table.Cell().Padding(3).Text(line.Description).FontSize(9);
                        table.Cell().Padding(3).AlignRight().Text(line.Amount.ToString("N2")).FontSize(9);
                    }
                });
            }

            column.Item().PaddingTop(6).BorderTop(1).PaddingTop(6).Row(row =>
            {
                row.RelativeItem().Text("Cəmi ödənilib:").FontSize(11).Bold();
                row.ConstantItem(100).AlignRight().Text($"{data.Amount:N2} ₼").FontSize(11).Bold();
            });
        });
    }

    private static void ComposeNote(IContainer container, PaymentReceiptData data)
    {
        if (string.IsNullOrWhiteSpace(data.Notes))
        {
            return;
        }

        container.PaddingTop(10).Text(text =>
        {
            text.Span("Qeyd:  ").FontSize(9);
            text.Span(data.Notes).FontSize(9);
        });
    }
}
