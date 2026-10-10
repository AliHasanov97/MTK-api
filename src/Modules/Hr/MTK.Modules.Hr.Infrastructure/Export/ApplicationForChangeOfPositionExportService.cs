using MTK.Modules.Hr.Application.Abstractions.Organization;
using MTK.Modules.Hr.Domain.ApplicationsForChangeOfPosition;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.JobApplications;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MTK.Modules.Hr.Infrastructure.Export;

internal sealed class ApplicationForChangeOfPositionExportService
{
    private readonly IOrganizationInfo _organization;

    private readonly IApplicationForChangeOfPositionRepository _repository;

    public ApplicationForChangeOfPositionExportService(IApplicationForChangeOfPositionRepository repository,
        IOrganizationInfo organization)
    {
        _organization = organization;
        _repository = repository;
    }

    public async Task<(MemoryStream Stream, int Index)> ExportToPdfAsync(
        Guid applicationId,
        CancellationToken cancellationToken = default)
    {
        var app = await _repository.GetByIdDefaultAsync(applicationId, cancellationToken)
            ?? throw new InvalidOperationException($"ApplicationForChangeOfPosition {applicationId} not found.");

        var stream = new MemoryStream();

        // Data extraction
        var employee = app.Employee!;
        var fullName = $"{employee.Name} {employee.Surname} {employee.FathersName}";
        var gender = employee.Gender == Gender.Male ? "oğlu" : "qızı";
        var companyName = _organization.Name ?? "";
        var directorName = _organization.Director ?? "";
        var currentPosition = app.CurrentJob?.Name ?? "";
        var newPosition = app.NewJob?.Name ?? "";
        var setDate = app.SetDate;
        var yearSuffix = GetYearSuffix(setDate.Year);

        var directorLine = string.IsNullOrWhiteSpace(directorName)
            ? "____________________________"
            : directorName;

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

                        // Field 1: Current position + "vəzifəsində çalışan"
                        appCol.Item().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text(currentPosition.ToLowerInvariant()).Italic();
                                col.Item().PaddingTop(2).Height(14).BorderBottom(0.5f).BorderColor(Colors.Black);
                            });
                            row.AutoItem().PaddingLeft(4).AlignBottom().Text("vəzifəsində çalışan");
                        });
                        appCol.Item().PaddingTop(2).AlignCenter().Text("(işçinin vəzifəsi)").FontSize(10);

                        appCol.Item().PaddingTop(10);

                        // Field 2: Full name
                        appCol.Item().Text($"{fullName} {gender}").Italic();
                        appCol.Item().PaddingTop(2).Height(14).BorderBottom(0.5f).BorderColor(Colors.Black);
                        appCol.Item().PaddingTop(2).AlignCenter().Text("(soyad, ad, ata adı)").FontSize(10);

                        appCol.Item().PaddingTop(10);

                        // Field 3: "tərəfindən"
                        appCol.Item().Text("tərəfindən");
                    });

                    column.Item().PaddingTop(40);

                    // Title (center, bold) - without index number
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
                        text.Span("\t\tYazıb Sizə bildirirəm ki, ");
                        text.Span(setDate.ToString("dd.MM.yyyy")).Underline();
                        text.Span($"-{yearSuffix} il tarixindən ");
                        text.Span(newPosition.ToLowerInvariant()).Underline();
                        text.Span(" vəzifəsinə (peşəsinə) keçirilməyimə etiraz etmirəm.");
                    });

                    column.Item().PaddingTop(50);

                    // Date line
                    column.Item().Text(
                        $"\"{setDate.Day:D2}\"  {GetMonthName(setDate.Month)}  {setDate.Year}-{yearSuffix} il");

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
    /// Based on the last non-zero digit (or tens digit when last digit is 0).
    /// </summary>
    private static string GetYearSuffix(int year)
    {
        var lastDigit = year % 10;

        if (lastDigit != 0)
            return lastDigit switch
            {
                1 => "ci",   // bir  → i
                2 => "ci",   // iki  → i
                3 => "cü",   // üç   → ü
                4 => "cü",   // dörd → ö
                5 => "ci",   // beş  → e
                6 => "cı",   // altı → ı
                7 => "ci",   // yeddi → i
                8 => "ci",   // səkkiz → i
                9 => "cu",   // doqquz → u
                _ => "ci"
            };

        // Last digit is 0 – vowel harmony from the tens word
        return ((year / 10) % 10) switch
        {
            1 => "cu",   // on     → o
            2 => "ci",   // iyirmi → i
            3 => "cu",   // otuz   → u
            4 => "cı",   // qırx   → ı
            5 => "ci",   // əlli   → i
            6 => "cı",   // altmış → ı
            7 => "ci",   // yetmiş → i
            8 => "ci",   // səksən → ə
            9 => "cı",   // doxsan → a
            _ => "cü"    // yüz / min → ü / i  (fallback)
        };
    }

    private static string GetMonthName(int month) => month switch
    {
        1  => "yanvar",
        2  => "fevral",
        3  => "mart",
        4  => "aprel",
        5  => "may",
        6  => "iyun",
        7  => "iyul",
        8  => "avqust",
        9  => "sentyabr",
        10 => "oktyabr",
        11 => "noyabr",
        12 => "dekabr",
        _  => ""
    };
}
