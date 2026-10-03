using MTK.Modules.Payments.Application.Abstractions.Services.Export;
using MTK.Modules.Payments.Application.Contracts.Queries.GetContractById;
using MTK.Modules.Payments.Domain.Contracts;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MTK.Modules.Payments.Infrastructure.Export;

internal sealed class ContractExportService : IContractExportService
{
    private static readonly Dictionary<ContractStatus, string> StatusLabels = new()
    {
        [ContractStatus.Draft] = "Hazırlanır",
        [ContractStatus.Active] = "Qüvvədədir",
        [ContractStatus.Suspended] = "Dayandırılıb",
        [ContractStatus.Terminated] = "Ləğv edilib",
    };

    private static readonly Dictionary<BillingPeriod, string> BillingPeriodLabels = new()
    {
        [BillingPeriod.Monthly] = "Aylıq",
        [BillingPeriod.Quarterly] = "Rüblük",
        [BillingPeriod.Yearly] = "İllik",
        [BillingPeriod.OneTime] = "Birdəfəlik",
    };

    public MemoryStream ExportToPdf(ContractResponse contract)
    {
        var stream = new MemoryStream();

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Content().Column(column =>
                {
                    column.Spacing(14);

                    column.Item().Element(c => ComposeHeader(c, contract));
                    column.Item().Element(c => ComposeInfo(c, contract));
                    column.Item().Element(c => ComposeServices(c, contract));
                    column.Item().Element(c => ComposeNote(c, contract));
                });
            });
        }).GeneratePdf(stream);

        stream.Position = 0;
        return stream;
    }

    private static void ComposeHeader(IContainer container, ContractResponse contract)
    {
        container.Column(column =>
        {
            column.Spacing(4);

            column.Item().AlignCenter().Text($"MÜQAVİLƏ № {contract.Number}").FontSize(14).Bold();
            column.Item().AlignCenter().Text(contract.VendorName ?? "Tədarükçü təyin edilməyib").FontSize(11);
            column.Item().PaddingTop(6).BorderBottom(1);
        });
    }

    private static void ComposeInfo(IContainer container, ContractResponse contract)
    {
        container.Column(column =>
        {
            column.Spacing(4);

            column.Item().Row(row =>
            {
                row.RelativeItem().Text(text =>
                {
                    text.Span("Başlanğıc:  ").FontSize(10);
                    text.Span(contract.StartDate.ToString("dd.MM.yyyy")).FontSize(10).Bold();
                });
                row.RelativeItem().Text(text =>
                {
                    text.Span("Bitmə:  ").FontSize(10);
                    text.Span(contract.EndDate.ToString("dd.MM.yyyy")).FontSize(10).Bold();
                });
            });

            column.Item().Text(text =>
            {
                text.Span("Status:  ").FontSize(10);
                text.Span(StatusLabels.GetValueOrDefault(contract.Status, contract.Status.ToString())).FontSize(10).Bold();
            });

            column.Item().Row(row =>
            {
                row.RelativeItem().Text(text =>
                {
                    text.Span("Aylıq yük:  ").FontSize(10);
                    text.Span($"{contract.MonthlyAmount:N2} ₼").FontSize(10).Bold();
                });
                row.RelativeItem().Text(text =>
                {
                    text.Span("Dövr üzrə cəm:  ").FontSize(10);
                    text.Span($"{contract.TotalAmount:N2} ₼").FontSize(10).Bold();
                });
            });
        });
    }

    private static void ComposeServices(IContainer container, ContractResponse contract)
    {
        container.Column(column =>
        {
            column.Spacing(6);

            column.Item().Text("Xidmətlər").FontSize(11).Bold();

            if (contract.Services.Count == 0)
            {
                column.Item().Text("Xidmət əlavə edilməyib.").FontSize(9);
                return;
            }

            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(1);
                    columns.ConstantColumn(80);
                });

                table.Header(header =>
                {
                    header.Cell().BorderBottom(1).Padding(3).Text("Xidmət").FontSize(9).Bold();
                    header.Cell().BorderBottom(1).Padding(3).Text("Dövr").FontSize(9).Bold();
                    header.Cell().BorderBottom(1).Padding(3).AlignRight().Text("Qiymət (₼)").FontSize(9).Bold();
                });

                foreach (var service in contract.Services)
                {
                    table.Cell().Padding(3).Text(text =>
                    {
                        text.Span(service.Name).FontSize(9);
                        if (!string.IsNullOrWhiteSpace(service.Description))
                        {
                            text.Span($" — {service.Description}").FontSize(9).FontColor(Colors.Grey.Darken1);
                        }
                    });
                    table.Cell().Padding(3).Text(BillingPeriodLabels.GetValueOrDefault(
                        service.BillingPeriod, service.BillingPeriod.ToString())).FontSize(9);
                    table.Cell().Padding(3).AlignRight().Text(service.PeriodAmount.ToString("N2")).FontSize(9);
                }
            });
        });
    }

    private static void ComposeNote(IContainer container, ContractResponse contract)
    {
        if (string.IsNullOrWhiteSpace(contract.Note))
        {
            return;
        }

        container.PaddingTop(10).Text(text =>
        {
            text.Span("Qeyd:  ").FontSize(9);
            text.Span(contract.Note).FontSize(9);
        });
    }
}
