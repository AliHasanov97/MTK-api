using MTK.Modules.Hr.Application.Abstractions.Organization;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.JobApplications;
using MTK.Modules.Hr.Domain.UnpaidLeaveApplications;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MTK.Modules.Hr.Infrastructure.Export;

internal sealed class UnpaidLeaveApplicationExportService
{
    private readonly IOrganizationInfo _organization;

    private readonly IUnpaidLeaveApplicationRepository _repository;

    public UnpaidLeaveApplicationExportService(IUnpaidLeaveApplicationRepository repository,
        IOrganizationInfo organization)
    {
        _organization = organization;
        _repository = repository;
    }

    public async Task<(MemoryStream Stream, int Index)> ExportToPdfAsync(
        Guid applicationId,
        CancellationToken cancellationToken = default)
    {
        var app = await _repository.GetByIdWithDetailsAsync(applicationId, cancellationToken)
            ?? throw new InvalidOperationException($"UnpaidLeaveApplication {applicationId} not found.");

        var stream = new MemoryStream();

        // Data extraction
        var employee = app.Employee!;
        var fullName = $"{employee.Name} {employee.Surname} {employee.FathersName}";
        var gender = employee.Gender == Gender.Male ? "oğlu" : "qızı";
        var companyName = _organization.Name ?? "";
        var directorName = _organization.Director ?? "";
        var position = employee.Job?.Name ?? "";
        var createdDate = app.CreatedAt.DateTime;
        var yearSuffix = GetYearSuffix(createdDate.Year);

        var directorLine = string.IsNullOrWhiteSpace(directorName)
            ? "____________________________"
            : directorName;

        // Calculate days
        var startDate = app.StartDate.DateTime;
        var endDate = app.EndDate.DateTime;
        var totalDays = (endDate - startDate).Days + 1; // +1 to include both start and end days

        var startYearSuffix = GetYearSuffix(startDate.Year);
        var endYearSuffix = GetYearSuffix(endDate.Year);

        // PDF generation with QuestPDF
        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.MarginLeft(85);
                page.MarginRight(42);
                page.MarginTop(57);
                page.MarginBottom(57);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Times New Roman"));

                page.Content().Column(column =>
                {
                    column.Spacing(0);

                    // Header section (right-aligned, bold)
                    column.Item().PaddingLeft(193).Column(headerCol =>
                    {
                        headerCol.Spacing(2);
                        headerCol.Item().Text(text =>
                        {
                            text.Span($"{companyName} direktoru {directorLine}").Bold();
                        });
                    });

                    column.Item().PaddingTop(10);

                    // Applicant info (right section)
                    column.Item().PaddingLeft(193).Column(appCol =>
                    {
                        appCol.Spacing(0);

                        // Field 1: Position + "vəzifəsində çalışan"
                        appCol.Item().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().AlignCenter().Text(position.ToLowerInvariant()).Italic();
                                col.Item().PaddingTop(2).Height(14).BorderBottom(0.5f).BorderColor(Colors.Black);
                            });
                            row.AutoItem().PaddingLeft(4).AlignBottom().Text("vəzifəsində çalışan");
                        });
                        appCol.Item().PaddingTop(2).AlignCenter().Text("(işçinin vəzifəsi)").FontSize(10);

                        appCol.Item().PaddingTop(10);

                        // Field 2: Full name
                        appCol.Item().AlignCenter().Text($"{fullName} {gender}").Italic();
                        appCol.Item().PaddingTop(2).Height(14).BorderBottom(0.5f).BorderColor(Colors.Black);
                        appCol.Item().PaddingTop(2).AlignCenter().Text("(soyad, ad, ata adı)").FontSize(10);

                        appCol.Item().PaddingTop(10);

                        // Field 3: "tərəfindən"
                        appCol.Item().Text("tərəfindən");
                    });

                    column.Item().PaddingTop(40);

                    // Title (center, bold)
                    column.Item().AlignCenter().Text(text =>
                    {
                        text.Span("Ə R İ Z Ə").Bold();
                    });

                    column.Item().PaddingTop(20);

                    // Body paragraph (justified, double line-height, first line indent using tab)
                    column.Item().Text(text =>
                    {
                        text.DefaultTextStyle(s => s.LineHeight(2f));
                        text.Justify();
                        text.Span("\t\tXahiş edirəm mənə ");
                        text.Span($"{startDate.Day:D2}.{startDate.Month:D2}.{startDate.Year}-{startYearSuffix}").Underline();
                        text.Span(" tarixindən ");
                        text.Span($"{endDate.Day:D2}.{endDate.Month:D2}.{endDate.Year}-{endYearSuffix}").Underline();
                        text.Span(" tarixinə qədər ");
                        text.Span($"{totalDays}").Underline();
                        text.Span($" ({NumberToWordsAz(totalDays)}) ");
                        text.Span("təqvim günü müddətinə ödənişsiz məzuniyyət verilməsinə icazə verilməsini xahiş edirəm.");
                    });

                    column.Item().PaddingTop(50);

                    // Date line
                    column.Item().Text(
                        $"\"{createdDate.Day:D2}\"  {GetMonthName(createdDate.Month)}  {createdDate.Year}-{yearSuffix} il");

                    column.Item().PaddingTop(20);

                    // Signature section
                    column.Item().Row(row =>
                    {
                        row.AutoItem().Text("İmza:  ");

                        row.RelativeItem(4).Column(sig =>
                        {
                            sig.Item().Height(20).BorderBottom(1f);
                            sig.Item().PaddingTop(2).AlignCenter().Text("(imza)").FontSize(10);
                        });

                        row.AutoItem().PaddingHorizontal(6).Text("/");

                        row.RelativeItem(6).Column(name =>
                        {
                            name.Item().Height(20).BorderBottom(1f).AlignCenter().Text($"{fullName} {gender}").Italic();
                            name.Item().PaddingTop(2).AlignCenter().Text("(ad, soyad, ata adı)").FontSize(10);
                        });
                    });
                });
            });
        }).GeneratePdf(stream);

        stream.Position = 0;
        return (stream, app.ApplicationNumber);
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

    private static string GetMonthName(int month) => month switch
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