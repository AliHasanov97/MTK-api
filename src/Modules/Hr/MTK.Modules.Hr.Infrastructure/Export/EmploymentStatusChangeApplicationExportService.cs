using MTK.Modules.Hr.Application.Abstractions.Organization;
using System.Globalization;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.EmploymentStatusChangeApplications;
using MTK.Modules.Hr.Domain.JobApplications;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MTK.Modules.Hr.Infrastructure.Export;

internal sealed class EmploymentStatusChangeApplicationExportService
{
    private readonly IOrganizationInfo _organization;

    private readonly IEmploymentStatusChangeApplicationRepository _repository;

    public EmploymentStatusChangeApplicationExportService(IEmploymentStatusChangeApplicationRepository repository,
        IOrganizationInfo organization)
    {
        _organization = organization;
        _repository = repository;
    }

    public async Task<(MemoryStream Stream, int Index)> ExportToPdfAsync(
        Guid applicationId,
        CancellationToken cancellationToken = default)
    {
        var app = await _repository.GetByIdForExportAsync(applicationId, cancellationToken)
            ?? throw new InvalidOperationException($"EmploymentStatusChangeApplication {applicationId} not found.");

        var stream = new MemoryStream();

        // Data extraction
        var employee = app.Employee!;
        var fullName = $"{employee.Name} {employee.Surname} {employee.FathersName}";
        var genderParticle = employee.Gender == Gender.Male ? "oğlu" : "qızı";

        var companyName = _organization.Name ?? "";
        var directorName = _organization.Director ?? "";
        var position = employee.Job?.Name ?? "";

        var createdDate = app.CreatedAt;
        var yearSuffix = GetYearSuffix(createdDate.Year);

        var currentEmploymentTypeText = GetEmploymentTypeText(app.CurrentEmploymentType);
        var newEmploymentTypeText = GetEmploymentTypeText(app.NewEmploymentType);

        var directorLine = string.IsNullOrWhiteSpace(directorName)
            ? "____________________________"
            : directorName;

        // PDF generation
        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.MarginLeft(85);
                page.MarginRight(42);
                page.MarginTop(57);
                page.MarginBottom(57);
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
                                col.Item().AlignCenter().Text(position.ToLower(new CultureInfo("az-Latn-AZ"))).Italic();
                                col.Item().PaddingTop(2).Height(14).BorderBottom(0.5f).BorderColor(Colors.Black);
                            });
                            row.AutoItem().PaddingLeft(4).AlignBottom().Text("vəzifəsində çalışan");
                        });
                        appCol.Item().PaddingTop(2).AlignCenter().Text("(işçinin vəzifəsi)").FontSize(10);

                        appCol.Item().PaddingTop(10);

                        // Field 2: Full name
                        appCol.Item().AlignCenter().Text($"{fullName} {genderParticle}").Italic();
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

                    // Body paragraph
                    column.Item().Text(text =>
                    {
                        text.DefaultTextStyle(s => s.LineHeight(2f));
                        text.Justify();
                        text.Span($"\t\tYazıb Sizdən xahiş edirəm ki, ");
                        text.Span($"{createdDate:dd.MM.yyyy}").Underline();
                        text.Span($"-{yearSuffix} il tarixdən etibarən {currentEmploymentTypeText}dan {newEmploymentTypeText} vahidinə keçirilməyimə sərəncam verəsiniz.");
                    });
                    
                    column.Item().PaddingTop(50);

                    // Date line
                    column.Item().Text(
                        $"\"{createdDate.Day:D2}\"  {GetMonthNameAz(createdDate.Month)}  {createdDate.Year}-{yearSuffix} il");

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
                            name.Item().Height(20).BorderBottom(1f).AlignCenter().Text($"{fullName} {genderParticle}").Italic();
                            name.Item().PaddingTop(2).AlignCenter().Text("(ad, soyad, ata adı)").FontSize(10);
                        });
                    });
                });
            });
        }).GeneratePdf(stream);

        stream.Position = 0;
        return (stream, app.ApplicationNumber);
    }

    private static string GetEmploymentTypeText(EmploymentType employmentType)
    {
        return employmentType switch
        {
            EmploymentType.FullTime => "tam ştat",
            EmploymentType.HalfTime => "yarım ştat",
            _ => "tam ştat"
        };
    }

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

    private static string GetYearSuffix(int year)
    {
        int lastDigit = year % 10;
        int secondLast = (year / 10) % 10;

        return lastDigit switch
        {
            0 => secondLast switch
            {
                1 => "cu",
                2 => "ci",
                3 => "cü",
                4 => "cü",
                5 => "ci",
                6 => "cı",
                7 => "ci",
                8 => "ci",
                9 => "cu",
                _ => "cı"
            },
            1 or 2 or 5 or 7 or 8 => "ci",
            3 or 4 => "cü",
            6 => "cı",
            9 => "cu",
            _ => "ci"
        };
    }
}
