using System.Globalization;
using MTK.Modules.Payments.Application.Abstractions.Services.Export;
using MTK.Modules.Payments.Application.Contracts.Queries.GetContractById;
using MTK.Modules.Payments.Domain.Contracts;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MTK.Modules.Payments.Infrastructure.Export;

/// <summary>Contract PDF styled consistently with the monthly report and payment receipt.</summary>
internal sealed class ContractExportService : IContractExportService
{
    private const string Navy = "#0F2A43";
    private const string Accent = "#1D4E89";
    private const string Rule = "#D7E0EA";
    private const string Muted = "#5B6B7C";
    private const string TextColor = "#1E293B";
    private const string SoftBackground = "#F7F9FC";
    private const string Green = "#067647";
    private const string Amber = "#B54708";

    private static readonly CultureInfo Az = CultureInfo.GetCultureInfo("az-Latn-AZ");

    private static readonly IReadOnlyDictionary<ContractStatus, string> StatusLabels =
        new Dictionary<ContractStatus, string>
        {
            [ContractStatus.Draft] = "Layihə",
            [ContractStatus.Active] = "Qüvvədədir",
            [ContractStatus.Suspended] = "Dayandırılıb",
            [ContractStatus.Terminated] = "Ləğv edilib",
        };

    private static readonly IReadOnlyDictionary<BillingPeriod, string> BillingPeriodLabels =
        new Dictionary<BillingPeriod, string>
        {
            [BillingPeriod.Monthly] = "Aylıq",
            [BillingPeriod.Quarterly] = "Rüblük",
            [BillingPeriod.Yearly] = "İllik",
            [BillingPeriod.OneTime] = "Birdəfəlik",
        };

    public MemoryStream ExportToPdf(ContractResponse contract)
    {
        var stream = new MemoryStream();

        Document.Create(document => document.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(34);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(style => style
                .FontFamily("Times New Roman")
                .FontSize(9)
                .FontColor(TextColor));

            page.Footer().PaddingTop(8).Row(row =>
            {
                row.RelativeItem().Text($"Müqavilə № {contract.Number} · PDF sənədi")
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
                column.Spacing(10);
                ComposeHeader(column, contract);
                ComposePartiesAndTerm(column, contract);
                ComposeServices(column, contract);
                ComposeNote(column, contract);
            });
        })).GeneratePdf(stream);

        stream.Position = 0;
        return stream;
    }

    private static void ComposeHeader(ColumnDescriptor column, ContractResponse contract)
    {
        column.Item().Row(row =>
        {
            row.RelativeItem().Column(title =>
            {
                title.Item().Text("MTK · MÜQAVİLƏ MƏLUMATLARI")
                    .FontSize(8).Bold().FontColor(Accent).LetterSpacing(0.06f);
                title.Item().PaddingTop(3).Text("MÜQAVİLƏ")
                    .FontSize(22).Bold().FontColor(Navy);
                title.Item().Text($"Müqavilə № {contract.Number}")
                    .FontSize(11).Bold().FontColor(TextColor);
            });

            row.ConstantItem(115).AlignRight().Border(0.7f).BorderColor(Rule)
                .Background(StatusBackground(contract)).Padding(10).Column(status =>
                {
                    status.Item().AlignRight().Text("STATUS")
                        .FontSize(7).Bold().FontColor(Muted).LetterSpacing(0.04f);
                    status.Item().PaddingTop(4).AlignRight().Text(StatusLabel(contract))
                        .FontSize(10).Bold().FontColor(StatusColor(contract));
                });
        });

        column.Item().PaddingTop(3).LineHorizontal(1.4f).LineColor(Navy);
    }

    private static void ComposePartiesAndTerm(ColumnDescriptor column, ContractResponse contract)
    {
        Section(column, "TƏRƏFLƏR VƏ MÜDDƏT");

        column.Item().Row(row =>
        {
            row.RelativeItem(1.25f).Border(0.7f).BorderColor(Rule).Background(SoftBackground)
                .Padding(11).Column(party =>
                {
                    string partyType = contract.VendorId.HasValue ? "TƏDARÜKÇÜ" : "MÜŞTƏRİ";
                    string partyName = contract.VendorName ?? contract.CustomerName ?? "Tərəf təyin edilməyib";
                    party.Item().Text("MÜQAVİLƏ TƏRƏFİ")
                        .FontSize(7).Bold().FontColor(Muted).LetterSpacing(0.04f);
                    party.Item().PaddingTop(4).Text(partyName)
                        .FontSize(12).Bold().FontColor(Navy);
                    party.Item().PaddingTop(3).Text(partyType)
                        .FontSize(7).Bold().FontColor(Accent).LetterSpacing(0.03f);
                });

            row.ConstantItem(10);
            row.RelativeItem().Border(0.7f).BorderColor(Rule).Padding(11).Column(term =>
            {
                term.Item().Text("MÜQAVİLƏNİN QÜVVƏDƏ OLMA MÜDDƏTİ")
                    .FontSize(7).Bold().FontColor(Muted).LetterSpacing(0.03f);
                term.Item().PaddingTop(6).Row(dates =>
                {
                    dates.RelativeItem().Column(start =>
                    {
                        start.Item().Text("Başlanma tarixi").FontSize(7).FontColor(Muted);
                        start.Item().PaddingTop(2).Text(Date(contract.StartDate))
                            .FontSize(10).Bold().FontColor(Navy);
                    });
                    dates.RelativeItem().Column(end =>
                    {
                        end.Item().Text("Bitmə tarixi").FontSize(7).FontColor(Muted);
                        end.Item().PaddingTop(2).Text(Date(contract.EndDate))
                            .FontSize(10).Bold().FontColor(Navy);
                    });
                });
            });
        });

        column.Item().Row(row =>
        {
            row.RelativeItem().Background("#EFF6FF").Padding(10).Column(metric =>
            {
                metric.Item().Text("AKTİV AYLIQ XİDMƏTLƏR ÜZRƏ")
                    .FontSize(7).Bold().FontColor(Accent).LetterSpacing(0.03f);
                metric.Item().PaddingTop(4).Text(Money(contract.MonthlyAmount))
                    .FontSize(15).Bold().FontColor(Navy);
            });
            row.ConstantItem(8);
            row.RelativeItem().Background(SoftBackground).Padding(10).Column(metric =>
            {
                metric.Item().Text("MÜQAVİLƏDƏKİ XİDMƏTLƏR")
                    .FontSize(7).Bold().FontColor(Muted).LetterSpacing(0.03f);
                metric.Item().PaddingTop(4).Text(contract.Services.Count.ToString(Az))
                    .FontSize(15).Bold().FontColor(Navy);
            });
        });
    }

    private static void ComposeServices(ColumnDescriptor column, ContractResponse contract)
    {
        Section(column, "XİDMƏTLƏR VƏ QİYMƏTLƏR");

        if (contract.Services.Count == 0)
        {
            column.Item().Border(0.7f).BorderColor(Rule).Background(SoftBackground).Padding(12)
                .Text("Müqaviləyə xidmət əlavə edilməyib.")
                .FontSize(9).FontColor(Muted);
            return;
        }

        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(3.4f);
                columns.RelativeColumn(1.1f);
                columns.RelativeColumn(1.15f);
                columns.RelativeColumn(1.2f);
                columns.ConstantColumn(58);
            });

            table.Header(header =>
            {
                header.Cell().Element(HeaderCell).Text("Xidmət və şərtlər");
                header.Cell().Element(HeaderCell).Text("Hesablama dövrü");
                header.Cell().Element(HeaderCell).AlignRight().Text("Vahid qiyməti");
                header.Cell().Element(HeaderCell).AlignRight().Text("Dövr üzrə məbləğ");
                header.Cell().Element(HeaderCell).AlignCenter().Text("Status");
            });

            foreach (var service in contract.Services)
            {
                table.Cell().Element(BodyCell).Column(details =>
                {
                    details.Item().Text(service.Name).FontSize(8.5f).Bold().FontColor(Navy);
                    if (!string.IsNullOrWhiteSpace(service.Description))
                        details.Item().PaddingTop(2).Text(service.Description).FontSize(7).FontColor(Muted);

                    var servicePeriod = ServicePeriod(service);
                    if (servicePeriod is not null)
                        details.Item().PaddingTop(3).Text(servicePeriod).FontSize(6.5f).FontColor(Muted);

                    if (service.PaymentTermDays is { } paymentTermDays)
                        details.Item().PaddingTop(2).Text($"Ödəniş müddəti: {paymentTermDays} gün")
                            .FontSize(6.5f).FontColor(Muted);
                });

                table.Cell().Element(BodyCell).Text(BillingPeriodLabel(service.BillingPeriod));
                table.Cell().Element(BodyCell).AlignRight().Text(Money(service.UnitPrice));
                table.Cell().Element(BodyCell).AlignRight().Text(Money(service.PeriodAmount)).Bold();
                table.Cell().Element(BodyCell).AlignCenter()
                    .Text(service.IsActive ? "Aktiv" : "Dayandırılıb")
                    .FontSize(7).FontColor(service.IsActive ? Green : Muted);
            }
        });

        column.Item().PaddingTop(3).Text(
                "Dövr üzrə məbləğ hər xidmət üçün göstərilən hesablama dövrünə aiddir. " +
                "Aylıq cəm yalnız aktiv aylıq xidmətləri əhatə edir.")
            .FontSize(7).FontColor(Muted).Italic();
    }

    private static void ComposeNote(ColumnDescriptor column, ContractResponse contract)
    {
        if (string.IsNullOrWhiteSpace(contract.Note))
            return;

        Section(column, "ƏLAVƏ QEYD");
        column.Item().Border(0.7f).BorderColor(Rule).Background(SoftBackground)
            .Padding(10).Text(contract.Note).FontSize(8.5f).LineHeight(1.3f);
    }

    private static void Section(ColumnDescriptor column, string title)
    {
        column.Item().PaddingTop(3).Text(title).FontSize(8).Bold().FontColor(Navy)
            .LetterSpacing(0.04f);
        column.Item().PaddingTop(2).LineHorizontal(0.7f).LineColor(Rule);
    }

    private static IContainer HeaderCell(IContainer container) => container
        .Background(Navy).PaddingVertical(7).PaddingHorizontal(6)
        .DefaultTextStyle(style => style.FontSize(7).Bold().FontColor(Colors.White));

    private static IContainer BodyCell(IContainer container) => container
        .BorderBottom(0.5f).BorderColor(Rule).PaddingVertical(7).PaddingHorizontal(6)
        .DefaultTextStyle(style => style.FontSize(7.5f).FontColor(TextColor));

    private static string StatusLabel(ContractResponse contract) => contract.IsExpired
        ? "Müddəti bitib"
        : StatusLabels.GetValueOrDefault(contract.Status, contract.Status.ToString());

    private static string StatusColor(ContractResponse contract) => contract.IsActive
        ? Green
        : contract.Status == ContractStatus.Draft ? Accent : Amber;

    private static string StatusBackground(ContractResponse contract) => contract.IsActive
        ? "#ECFDF3"
        : contract.Status == ContractStatus.Draft ? "#EFF6FF" : "#FFFAEB";

    private static string BillingPeriodLabel(BillingPeriod period) =>
        BillingPeriodLabels.GetValueOrDefault(period, period.ToString());

    private static string? ServicePeriod(ContractServiceResponse service)
    {
        if (service.ServiceStartDate is null && service.ServiceEndDate is null)
            return null;

        string start = service.ServiceStartDate is { } serviceStart ? Date(serviceStart) : "Başlanğıc göstərilməyib";
        string end = service.ServiceEndDate is { } serviceEnd ? Date(serviceEnd) : "Bitmə tarixi göstərilməyib";
        return $"Xidmət müddəti: {start} – {end}";
    }

    private static string Date(DateTimeOffset value) => value.ToString("dd.MM.yyyy", Az);

    private static string Money(decimal amount) => $"{amount.ToString("N2", Az)} ₼";
}
