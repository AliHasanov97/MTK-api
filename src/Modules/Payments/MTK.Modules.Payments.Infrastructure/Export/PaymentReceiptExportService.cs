using System.Globalization;
using MTK.Modules.Payments.Application.Abstractions.Services.Export;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Parties;
using MTK.Modules.Payments.Domain.Payments;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MTK.Modules.Payments.Infrastructure.Export;

/// <summary>
/// Matches the complex's existing paper receipt wording (a handwritten/stamped "Qəbz"
/// template), not a generic invoice layout — residents expect the same phrasing they
/// already know from the paper version, just typeset cleanly. Only the company name is
/// fixed here (a single-complex assumption); the building address and the signing
/// issuer's name/title are resolved per-payment by the caller (see
/// ExportPaymentReceiptQueryHandler), since the issuer isn't always the same person or
/// role — could be the Komendant or the Xəzinədar, whoever actually recorded it.
/// </summary>
internal sealed class PaymentReceiptExportService : IPaymentReceiptExportService
{
    private const string CompanyName = "Yusifoğlu İnşaat";
    private const string DefaultIssuerTitle = "Komendantı";

    private static readonly string AccentColor = Colors.Blue.Darken2;
    private static readonly string RuleColor = Colors.Grey.Lighten1;

    private static readonly string[] AzMonths =
    [
        "Yanvar", "Fevral", "Mart", "Aprel", "May", "İyun",
        "İyul", "Avqust", "Sentyabr", "Oktyabr", "Noyabr", "Dekabr",
    ];

    public MemoryStream ExportToPdf(PaymentReceiptData data)
    {
        var stream = new MemoryStream();

        Document.Create(container =>
        {
            container.Page(page =>
            {
                // A pre-printed receipt-pad slip (a third of an A4 sheet, landscape) —
                // matches the complex's actual paper "Qəbz" stock, not a full A4/A5 page.
                page.Size(new PageSize(210, 99, Unit.Millimetre));
                page.Margin(0);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11f).FontFamily("Times New Roman").FontColor(Colors.Grey.Darken4));

                page.Content()
                    .Padding(8)
                    .Border(1.2f)
                    .BorderColor(AccentColor)
                    .Padding(9)
                    .Column(column =>
                    {
                        column.Spacing(3.5f);

                        // ---- Header: company eyebrow, title, receipt no./date on one row ----
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(head =>
                            {
                                head.Item().Text(CompanyName.ToUpper(new CultureInfo("az-AZ")))
                                    .FontSize(8f).FontColor(AccentColor).LetterSpacing(0.05f);
                                head.Item().Text("Qəbz").FontSize(20).Bold().FontColor(Colors.Grey.Darken4);
                            });

                            row.ConstantItem(95).AlignRight().Column(meta =>
                            {
                                meta.Item().AlignRight().Text($"№ {ShortRef(data.PaymentId)}")
                                    .FontSize(9f).FontColor(Colors.Grey.Darken2);
                                meta.Item().AlignRight().Text(
                                    $"«{data.PaymentDate.Day}» {AzMonths[data.PaymentDate.Month - 1]} {data.PaymentDate.Year}-ci il")
                                    .FontSize(9.5f);
                            });
                        });

                        column.Item().LineHorizontal(0.75f).LineColor(RuleColor);

                        // ---- Payer ----
                        column.Item().Text(text =>
                        {
                            text.Span("Verilir: ").FontColor(Colors.Grey.Darken2);
                            text.Span(data.PayerName ?? "—").Bold().FontSize(13f);
                        });

                        // ---- Body ----
                        if (data.PartyType == PartyType.Owner)
                        {
                            ComposeOwnerBody(column, data);
                        }
                        else
                        {
                            ComposeVendorBody(column, data);
                        }

                        if (!string.IsNullOrWhiteSpace(data.Notes))
                        {
                            column.Item().Text($"Qeyd: {data.Notes}")
                                .FontSize(9f).Italic().FontColor(Colors.Grey.Darken1);
                        }

                        // Normal top-down flow (not pinned to the page bottom): an
                        // itemised bulk/toplu payment can grow a few extra lines, and
                        // forcing the signature to the page's bottom edge would push it
                        // onto a second page instead of just flowing below the longer body.
                        column.Item().PaddingTop(5).Row(row =>
                        {
                            row.RelativeItem().AlignBottom().Text(MethodLabel(data.PaymentMethod))
                                .FontSize(9.5f).FontColor(Colors.Grey.Darken2);

                            row.ConstantItem(150).AlignRight().Column(sig =>
                            {
                                sig.Spacing(1f);
                                sig.Item().AlignRight().Text($"\"{CompanyName}\" MTK-nın").FontSize(9.5f);
                                sig.Item().AlignRight().Text($"{data.IssuerTitle ?? DefaultIssuerTitle}:").FontSize(9.5f);
                                sig.Item().PaddingTop(7).AlignRight().Text(data.IssuerName ?? "—")
                                    .FontSize(12f).Bold();
                                sig.Item().PaddingTop(1).Width(130).LineHorizontal(0.5f).LineColor(RuleColor);
                            });
                        });
                    });
            });
        }).GeneratePdf(stream);

        stream.Position = 0;
        return stream;
    }

    // "Ondan ötrü ki, [ünvan] ünvanında yerləşən "[şirkət]" MTK-nın inşa etdiyi yaşayış
    // binasında [N] saylı mənzilə/qaraja görə [ay] [il]-ci il üçün [məbləğ] AZN kommunal
    // haqqı ödənişdir." — the exact wording of the paper receipt, used whenever the
    // payment settles a single unit for a single, unambiguous period. A bulk/toplu
    // payment — several months at once, OR (crucially) several DIFFERENT units at
    // once, e.g. part clearing an apartment's debt and the rest a garage's — is NOT
    // squeezed into that one-unit/one-month sentence: it gets an itemised line per
    // unit+period instead, so the receipt never claims the whole amount went to one
    // unit when it actually didn't.
    private static void ComposeOwnerBody(ColumnDescriptor column, PaymentReceiptData data)
    {
        // No settled charges at all (Lines empty) means this payment hasn't actually been
        // applied to anything — it's sitting as a prepayment/advance on the owner's account
        // (same case the web UI labels "avans kimi saxlanılır"). Any Apartment/Garage number
        // or period the handler resolved here is only the payment's original TARGET, not
        // where the money actually went — asserting a unit+month would misstate the receipt.
        if (data.Lines.Count == 0)
        {
            column.Item().Text(
                $"Ondan ötrü ki, \"{CompanyName}\" MTK-nın hesabına {Money(data.Amount)} AZN ödəniş daxil olub. " +
                "Bu məbləğ hələ heç bir haqqa tətbiq olunmayıb və sahibin hesabında avans kimi saxlanılır.")
                .Justify().LineHeight(1.08f);
            return;
        }

        // A line only carries its own PropertyLabel when the payment settled more than
        // one unit — see ExportPaymentReceiptQueryHandler. Grouping by (label, period)
        // catches both bulk shapes at once: several units, or several months of the same unit.
        var groups = data.Lines
            .GroupBy(l => (l.PropertyLabel, l.Period))
            .ToList();
        bool isBulkPayment = groups.Count > 1;

        // data.Amount is what the owner PAID, not necessarily what got APPLIED — FIFO can
        // leave a remainder with no open charge to cover (same "qalıq ... avans kimi
        // saxlanılır" the web UI shows). The sentence below must only claim the portion
        // this unit/period actually received; the leftover gets its own note further down.
        var appliedTotal = data.Lines.Sum(l => l.Amount);
        var remainder = data.Amount - appliedTotal;
        bool hasRemainder = remainder > 0.004m;

        var unitWord = data.PropertyType == PropertyType.Garage ? "qaraja" : "mənzilə";
        var unitClause = isBulkPayment
            ? "əmlaklara görə"
            : data.PropertyNumber is not null
                ? $"{data.PropertyNumber} saylı {unitWord} görə"
                : unitWord + " görə";

        column.Item().Text(
            $"Ondan ötrü ki, {data.BuildingAddress ?? "—"} ünvanında yerləşən \"{CompanyName}\" MTK-nın inşa etdiyi " +
            $"yaşayış binasında {unitClause}").Justify().LineHeight(1.08f);

        if (!isBulkPayment)
        {
            var (periodMonth, periodYear) = ResolvePeriod(data);
            column.Item().Text(
                $"{AzMonths[periodMonth - 1]} {periodYear}-ci il üçün {Money(appliedTotal)} AZN kommunal haqqı ödənişdir.");
        }
        else
        {
            // Toplu ödəniş: hər əmlak/dövr öz sətrində, cəmi aydın görünsün deyə.
            column.Item().Text("aşağıdakılara görə:").Justify();

            foreach (var group in groups)
            {
                var (propertyLabel, period) = group.Key;
                var periodLabel = FormatPeriodLabel(period);
                var label = propertyLabel is not null && periodLabel is not null
                    ? $"{propertyLabel}, {periodLabel}"
                    : propertyLabel ?? periodLabel ?? group.First().Description;

                var amount = group.Sum(l => l.Amount);
                column.Item().PaddingLeft(10).Text($"— {label}: {Money(amount)} AZN").FontSize(10f);
            }

            column.Item().Text($"Cəmi: {Money(appliedTotal)} AZN kommunal haqqı ödənişdir.").Bold();
        }

        if (hasRemainder)
        {
            column.Item().Text($"Qalan {Money(remainder)} AZN sahibin hesabında avans kimi saxlanılır.")
                .Italic().FontColor(Colors.Grey.Darken2);
        }
    }

    private static string? FormatPeriodLabel(string? period)
    {
        if (period is not { Length: 7 }
            || !int.TryParse(period.AsSpan(0, 4), out var year)
            || !int.TryParse(period.AsSpan(5, 2), out var month))
        {
            return null;
        }

        return $"{AzMonths[month - 1]} {year}-ci il";
    }

    private static void ComposeVendorBody(ColumnDescriptor column, PaymentReceiptData data)
    {
        // Same unapplied-advance case as ComposeOwnerBody: nothing was actually settled yet.
        if (data.Lines.Count == 0)
        {
            column.Item().Text(
                $"Ondan ötrü ki, \"{CompanyName}\" MTK-nın hesabına {Money(data.Amount)} AZN ödəniş daxil olub. " +
                "Bu məbləğ hələ heç bir xərcə tətbiq olunmayıb və tədarükçünün hesabında avans kimi saxlanılır.")
                .Justify().LineHeight(1.08f);
            return;
        }

        var appliedTotal = data.Lines.Sum(l => l.Amount);
        var remainder = data.Amount - appliedTotal;
        bool hasRemainder = remainder > 0.004m;

        var description = data.Lines.Count == 1
            ? data.Lines.First().Description
            : "aşağıdakı xidmətlərə görə";

        column.Item().Text($"Ondan ötrü ki, {description} {Money(appliedTotal)} AZN")
            .Justify().LineHeight(1.08f);

        if (data.Lines.Count > 1)
        {
            foreach (var line in data.Lines)
            {
                column.Item().PaddingLeft(10).Text($"— {line.Description}: {Money(line.Amount)} AZN").FontSize(10f);
            }
        }

        column.Item().Text("ödənişdir.");

        if (hasRemainder)
        {
            column.Item().Text($"Qalan {Money(remainder)} AZN tədarükçünün hesabında avans kimi saxlanılır.")
                .Italic().FontColor(Colors.Grey.Darken2);
        }
    }

    // "yyyy-MM" when every settled charge shares one billing period; otherwise the
    // payment's own date — still a concrete, meaningful month for the receipt to name.
    private static (int Month, int Year) ResolvePeriod(PaymentReceiptData data)
    {
        if (data.Period is { Length: 7 } period
            && int.TryParse(period.AsSpan(0, 4), out var year)
            && int.TryParse(period.AsSpan(5, 2), out var month))
        {
            return (month, year);
        }

        return (data.PaymentDate.Month, data.PaymentDate.Year);
    }

    private static string Money(decimal amount) => amount.ToString("N2", CultureInfo.InvariantCulture);

    private static string ShortRef(Guid id) => id.ToString()[..8].ToUpperInvariant();

    private static string MethodLabel(PaymentMethod method) => method switch
    {
        PaymentMethod.Cash => "Nağd ödəniş",
        PaymentMethod.BankTransfer => "Bank köçürməsi",
        PaymentMethod.Card => "Kartla ödəniş",
        _ => method.ToString(),
    };
}
