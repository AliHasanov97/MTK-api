using MTK.Modules.Hr.Application.Abstractions.Organization;
using MTK.Modules.Hr.Domain.BonusOrders;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.JobApplications;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MTK.Modules.Hr.Infrastructure.Export;

internal sealed class BonusOrderExportService
{
    private readonly IOrganizationInfo _organization;

    private readonly IBonusOrderRepository _repository;

    public BonusOrderExportService(IBonusOrderRepository repository,
        IOrganizationInfo organization)
    {
        _organization = organization;
        _repository = repository;
    }

    public async Task<(MemoryStream Stream, int Index)> ExportToPdfAsync(
        Guid bonusOrderId,
        CancellationToken cancellationToken = default)
    {
        var bonusOrder = await _repository.GetByIdForExportAsync(bonusOrderId, cancellationToken)
            ?? throw new InvalidOperationException($"BonusOrder {bonusOrderId} not found.");

        var stream = new MemoryStream();

        // Data extraction
        var employee = bonusOrder.Employee!;
        var supervisor = bonusOrder.OrderExecutionSupervisor!;
        var fullName = $"{employee.Name} {employee.Surname} {employee.FathersName}";
        var gender = employee.Gender == Gender.Male ? "oğlu" : "qızı";
        var genderSuffixNun = employee.Gender == Gender.Male ? "oğlunun" : "qızının";
        var genderSuffixNa = employee.Gender == Gender.Male ? "oğluna" : "qızına";
        var supervisorFullName = $"{supervisor.Name} {supervisor.Surname} {supervisor.FathersName}";
        var supervisorGenderSuffixNa = supervisor.Gender == Gender.Male ? "oğluna" : "qızına";
        var companyName = _organization.Name;
        var directorName = _organization.Director;
        var jobName = employee.Job?.Name;
        var bonusQuantity = bonusOrder.BonusQuantity;
        var bonusQuantityWords = NumberToWordsAz(bonusQuantity);
        var salaryMonthName = GetMonthNameAz(bonusOrder.SalaryMonth);
        var salaryYear = bonusOrder.SalaryYear;
        var orderNumber = bonusOrder.OrderNumber;
        var createdDate = bonusOrder.CreatedAt;
        var createdYearSuffix = GetYearSuffix(createdDate.Year);

        // PDF generation with QuestPDF
        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.MarginLeft(85);
                page.MarginRight(85);
                page.MarginTop(57);
                page.MarginBottom(57);
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

                    column.Item().PaddingTop(20);

                    // "üzrə" - center
                    column.Item().AlignCenter().Text("üzrə").Bold();

                    column.Item().PaddingTop(20);

                    // Order number
                    column.Item().AlignCenter().Text(text =>
                    {
                        text.Span("Əmr №").Bold();
                        text.Span($"{orderNumber:D4}").Underline().Bold();
                    });

                    column.Item().PaddingTop(20);

                    // Date
                    column.Item().AlignRight().Text(text =>
                    {
                        text.Span($"{createdDate:dd}.{createdDate:MM}.{createdDate:yyyy}-{createdYearSuffix} il").Bold();
                    });

                    column.Item().PaddingTop(30);

                    // Title
                    column.Item().Text(text =>
                    {
                        text.Span("\"Mükafatlandırma barədə\"").Bold();
                    });

                    column.Item().PaddingTop(20);

                    // Main intro paragraph
                    column.Item().Text(text =>
                    {
                        text.DefaultTextStyle(s => s.LineHeight(1.8f));
                        text.Justify();
                        text.Span($"{companyName}-nin ");
                        text.Span($"{jobName?.ToLowerInvariant()}").Underline();
                        text.Span(" vəzifəsində çalışan ");
                        text.Span($"{fullName} {genderSuffixNun}").Underline();
                        text.Span(" mükafatlandırılması məqsədilə:");
                    });

                    column.Item().PaddingTop(20);

                    // "ƏMR EDİRƏM" header
                    column.Item().AlignCenter().Text("ƏMR EDİRƏM").Bold();

                    column.Item().PaddingTop(20);

                    // Paragraph 1
                    column.Item().Text(text =>
                    {
                        text.DefaultTextStyle(s => s.LineHeight(1.8f));
                        text.Justify();
                        text.Span("1. ").Bold();
                        text.Span($"Cəmiyyətin ");
                        text.Span($"{jobName?.ToLowerInvariant()}").Underline();
                        text.Span(" vəzifəsində çalışan ");
                        text.Span($"{fullName} {gender}").Underline();
                        text.Span(" əmək funksiyalarını yüksək səviyyədə yerinə yetirdiyi üçün ");
                        text.Span($"{bonusQuantity}").Underline();
                        text.Span($" ({bonusQuantityWords})");
                        text.Span(" manat məbləğində mükafatlandırılsın.");
                    });

                    column.Item().PaddingTop(10);

                    // Paragraph 2
                    column.Item().Text(text =>
                    {
                        text.DefaultTextStyle(s => s.LineHeight(1.8f));
                        text.Justify();
                        text.Span("2. ").Bold();
                        text.Span($"{fullName} {gender}na").Underline();
                        text.Span(" müəyyən olunmuş mükafat məbləği ");
                        text.Span($"{salaryMonthName}").Underline();
                        text.Span(" ayının əmək haqqı ilə birlikdə ödənilsin.");
                    });

                    column.Item().PaddingTop(10);

                    // Paragraph 3
                    column.Item().Text(text =>
                    {
                        text.DefaultTextStyle(s => s.LineHeight(1.8f));
                        text.Justify();
                        text.Span("3. ").Bold();
                        text.Span("Əmrin surəti mühasibatlığa təqdim olunsun.");
                    });

                    column.Item().PaddingTop(10);

                    // Paragraph 4
                    column.Item().Text(text =>
                    {
                        text.DefaultTextStyle(s => s.LineHeight(1.8f));
                        text.Justify();
                        text.Span("4. ").Bold();
                        text.Span("Əmrin icrasına nəzarət ");
                        text.Span($"{supervisorFullName} {supervisorGenderSuffixNa}").Underline();
                        text.Span(" tapşırılsın.");
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
                        row.RelativeItem().AlignRight().Text(directorName).Bold();
                    });

                    column.Item().PaddingTop(40);
                    
                });
            });
        }).GeneratePdf(stream);

        stream.Position = 0;
        return (stream, bonusOrder.OrderNumber);
    }

    /// <summary>
    /// Converts month number to Azerbaijani month name.
    /// </summary>
    private static string GetMonthNameAz(int month)
    {
        return month switch
        {
            1 => "yanvar",
            2 => "fevral",
            3 => "mart",
            4 => "aprel",
            5 => "may",
            6 => "iyun",
            7 => "iyul",
            8 => "avqust",
            9 => "sentyabr",
            10 => "oktyabr",
            11 => "noyabr",
            12 => "dekabr",
            _ => ""
        };
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
            words += NumberToWordsAz(thousands) + " min ";
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
