using MTK.Modules.Hr.Application.Abstractions.Organization;
using MTK.Modules.Hr.Domain.WorkOnNonWorkdayOrders;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MTK.Modules.Hr.Infrastructure.Export;

internal sealed class WorkOnNonWorkdayOrderExportService
{
    private readonly IOrganizationInfo _organization;

    private readonly IWorkOnNonWorkdayOrderRepository _repository;

    public WorkOnNonWorkdayOrderExportService(IWorkOnNonWorkdayOrderRepository repository,
        IOrganizationInfo organization)
    {
        _organization = organization;
        _repository = repository;
    }

    public async Task<(MemoryStream Stream, int Index)> ExportToPdfAsync(
        Guid workOnNonWorkdayOrderId,
        CancellationToken cancellationToken = default)
    {
        var order = await _repository.GetByIdDefaultAsync(workOnNonWorkdayOrderId, cancellationToken)
            ?? throw new InvalidOperationException($"WorkOnNonWorkdayOrder {workOnNonWorkdayOrderId} not found.");

        var stream = new MemoryStream();

        // Data extraction
        var companyName = _organization.Name ?? "XXXXX";
        var companyAdress = _organization.Address;
        var directorName = _organization.Director ?? "";
        var orderNumber = order.OrderNumber;
        var createdDate = order.CreatedAt;
        var startDate = order.StartDate;
        var endDate = order.EndDate ?? order.StartDate;
        // Calculate days count
        var daysCount = (endDate.Date - startDate.Date).Days + 1;
        var daysCountWords = NumberToWordsAz(daysCount);

        // PDF generation with QuestPDF
        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.MarginLeft(70);
                page.MarginRight(70);
                page.MarginTop(50);
                page.MarginBottom(50);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Times New Roman"));

                page.Content().Column(column =>
                {
                    column.Spacing(0);

                    // Header - Company name
                    column.Item().AlignCenter().Text(text =>
                    {
                        text.Span(companyName).Bold();
                    });

                    column.Item().PaddingTop(15);

                    // "üzrə" - center
                    column.Item().AlignCenter().Text("üzrə").Bold();

                    column.Item().PaddingTop(15);

                    // Order number
                    column.Item().AlignCenter().Text(text =>
                    {
                        text.Span("ƏMR №").Bold();
                        text.Span($"{orderNumber:D4}").Underline().Bold();
                    });

                    column.Item().PaddingTop(15);

                    // City and date row
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Text(companyAdress).Bold();
                        row.RelativeItem().AlignRight().Text(text =>
                        {
                            text.Span($"{createdDate:dd}.{createdDate:MM}.{createdDate:yyyy}").Underline();
                            text.Span("-");
                            text.Span(GetYearSuffix(createdDate.Year));
                            text.Span(" il");
                        });
                    });

                    column.Item().PaddingTop(20);

                    // Title paragraph
                    column.Item().Text(text =>
                    {
                        text.DefaultTextStyle(s => s.LineHeight(1.6f));
                        text.Justify();
                        text.Span($"\"{companyName}\"-də ").Bold();

                        if (daysCount == 1)
                        {
                            // Bir gün olduqda: "... tarixində 1(bir) təqvim günü ..."
                            text.Span($"{startDate:dd}.{startDate:MM}.{startDate:yyyy}").Underline();
                            text.Span("-");
                            text.Span(GetYearSuffix(startDate.Year));
                            text.Span(" il tarixində ");
                        }
                        else
                        {
                            // Çox gün olduqda: "... tarixdən ... tarixədək ..."
                            text.Span($"{startDate:dd}.{startDate:MM}.{startDate:yyyy}").Underline();
                            text.Span("-");
                            text.Span(GetYearSuffix(startDate.Year));
                            text.Span(" il tarixdən ");
                            text.Span($"{endDate:dd}.{endDate:MM}.{endDate:yyyy}").Underline();
                            text.Span("-");
                            text.Span(GetYearSuffix(endDate.Year));
                            text.Span(" il tarixədək ");
                        }

                        text.Span($"{daysCount}").Underline();
                        text.Span("(");
                        text.Span($"{daysCountWords}").Underline();
                        text.Span(") təqvim günü cəmiyyətin işçilərinin işə cəlb edilməsinə dair:");
                    });

                    column.Item().PaddingTop(20);

                    // "ƏMR edirəm" - center
                    column.Item().AlignCenter().Text("ƏMR edirəm").Bold();

                    column.Item().PaddingTop(20);

                    // Point 1
                    var dayWord = daysCount == 1 ? "günündə" : "günlərində";
                    column.Item().Text(text =>
                    {
                        text.DefaultTextStyle(s => s.LineHeight(1.6f));
                        text.Justify();
                        text.Span("1. ").Bold();
                        text.Span($"Cəmiyyətdə təxirəsalınmaz işlərin həyata keçirilməsi məqsədilə Azərbaycan Respublikasının Əmək Məcəlləsinin 109-cu maddəsinə əsasən \"İstehsal zərurəti\" ilə əlaqədar olaraq iş günü hesab edilməyən qeyri iş {dayWord} Cəmiyyətin işçilərinin işə çıxması təmin edilsin.");
                    });

                    column.Item().PaddingTop(10);

                    // Point 2
                    column.Item().Text(text =>
                    {
                        text.DefaultTextStyle(s => s.LineHeight(1.6f));
                        text.Justify();
                        text.Span("2. ").Bold();
                        text.Span($"Maliyyə şöbəsinə tapşırılsın ki, iş günü hesab edilməyən qeyri iş {dayWord} işçilərin əmək haqqları Azərbaycan Respublikasının Əmək Məcəlləsinin 164-cü maddələrinə əsasən, hər günə görə gündəlik vəzifə maaşının ikiqat məbləğində ödəniş edilsin.");
                    });

                    column.Item().PaddingTop(10);

                    // Point 3
                    column.Item().Text(text =>
                    {
                        text.DefaultTextStyle(s => s.LineHeight(1.6f));
                        text.Justify();
                        text.Span("3. ").Bold();
                        text.Span($"İnsan resursları şöbəsinə tapşırılsın ki, qeyri iş {dayWord} işləyən işçilərin razılıq ərizələri alınsın.");
                    });

                    column.Item().PaddingTop(10);

                    // Point 4
                    column.Item().Text(text =>
                    {
                        text.DefaultTextStyle(s => s.LineHeight(1.6f));
                        text.Justify();
                        text.Span("4. ").Bold();
                        text.Span("İşçilər əmrlə tanış edilsin.");
                    });

                    column.Item().PaddingTop(40);

                    // Company name at bottom
                    column.Item().Text(text =>
                    {
                        text.Span(companyName).Bold();
                    });

                    column.Item().PaddingTop(30);

                    // Director signature section
                    column.Item().Row(row =>
                    {
                        row.AutoItem().Text("Direktoru").Bold();
                        row.RelativeItem().AlignRight().Text($"{directorName}").Bold();
                    });
                });
            });
        }).GeneratePdf(stream);

        stream.Position = 0;
        return (stream, order.OrderNumber);
    }

    /// <summary>
    /// Returns the correct Azerbaijani vowel-harmony suffix for a given year.
    /// </summary>
    private static string GetYearSuffix(int year)
    {
        var lastDigit = year % 10;

        if (lastDigit != 0)
            return lastDigit switch
            {
                1 => "ci",
                2 => "ci",
                3 => "cü",
                4 => "cü",
                5 => "ci",
                6 => "cı",
                7 => "ci",
                8 => "ci",
                9 => "cu",
                _ => "ci"
            };

        return ((year / 10) % 10) switch
        {
            1 => "cu",
            2 => "ci",
            3 => "cu",
            4 => "cı",
            5 => "ci",
            6 => "cı",
            7 => "ci",
            8 => "ci",
            9 => "cı",
            _ => "cü"
        };
    }

    /// <summary>
    /// Converts a number to Azerbaijani words.
    /// </summary>
    private static string NumberToWordsAz(int number)
    {
        if (number == 0) return "sıfır";
        if (number < 0) return "mənfi " + NumberToWordsAz(-number);

        var words = "";

        if (number >= 1000)
        {
            var thousands = number / 1000;
            words += thousands == 1 ? "min " : NumberToWordsAz(thousands) + " min ";
            number %= 1000;
        }

        if (number >= 100)
        {
            var hundreds = number / 100;
            words += hundreds switch
            {
                1 => "yüz ",
                2 => "iki yüz ",
                3 => "üç yüz ",
                4 => "dörd yüz ",
                5 => "beş yüz ",
                6 => "altı yüz ",
                7 => "yeddi yüz ",
                8 => "səkkiz yüz ",
                9 => "doqquz yüz ",
                _ => ""
            };
            number %= 100;
        }

        if (number >= 10)
        {
            var tens = number / 10;
            words += tens switch
            {
                1 => "on ",
                2 => "iyirmi ",
                3 => "otuz ",
                4 => "qırx ",
                5 => "əlli ",
                6 => "altmış ",
                7 => "yetmiş ",
                8 => "səksən ",
                9 => "doxsan ",
                _ => ""
            };
            number %= 10;
        }

        if (number > 0)
        {
            words += number switch
            {
                1 => "bir ",
                2 => "iki ",
                3 => "üç ",
                4 => "dörd ",
                5 => "beş ",
                6 => "altı ",
                7 => "yeddi ",
                8 => "səkkiz ",
                9 => "doqquz ",
                _ => ""
            };
        }

        return words.Trim();
    }
}
