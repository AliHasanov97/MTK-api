using MTK.Modules.Hr.Application.Abstractions.Organization;
using System.Globalization;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.EmploymentStatusChangeOrders;
using MTK.Modules.Hr.Domain.JobApplications;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MTK.Modules.Hr.Infrastructure.Export;

internal sealed class EmploymentStatusChangeOrderExportService
{
    private readonly IOrganizationInfo _organization;

    private readonly IEmploymentStatusChangeOrderRepository _repository;

    public EmploymentStatusChangeOrderExportService(IEmploymentStatusChangeOrderRepository repository,
        IOrganizationInfo organization)
    {
        _organization = organization;
        _repository = repository;
    }

    public async Task<(MemoryStream Stream, int Index)> ExportToPdfAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await _repository.GetByIdForExportAsync(orderId, cancellationToken)
            ?? throw new InvalidOperationException($"EmploymentStatusChangeOrder {orderId} not found.");

        var stream = new MemoryStream();

        // Data extraction
        var employee = order.Employee!;
        var supervisor = order.OrderExecutionSupervisor!;
        
        var fullName = $"{employee.Name} {employee.Surname} {employee.FathersName}";
        var supervisorFullName = $"{supervisor.Name} {supervisor.Surname} {supervisor.FathersName}";

        var genderParticle = employee.Gender == Gender.Male ? "oğlu" : "qızı";
        var genderSuffixNun = employee.Gender == Gender.Male ? "oğlunun" : "qızının";
        var supervisorGenderParticle = supervisor.Gender == Gender.Male ? "oğlu" : "qızı";
        var supervisorGenderSuffixNun = supervisor.Gender == Gender.Male ? "oğlunun" : "qızının";

        var companyName = _organization.Name ?? "";
        var directorName = _organization.Director ?? "";
        var companyAddress = _organization.Address ?? "";
        var jobName = employee.Job?.Name ?? "";
        var supervisorJobName = supervisor.Job?.Name ?? "";

        var orderNumber = order.OrderNumber;
        var createdDate = order.CreatedAt;
        var createdYearSuffix = GetYearSuffix(createdDate.Year);

        var currentEmploymentTypeText = GetEmploymentTypeText(order.CurrentEmploymentType);
        var newEmploymentTypeText = GetEmploymentTypeText(order.NewEmploymentType);

        // PDF generation
        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.MarginLeft(85);
                page.MarginRight(85);
                page.MarginTop(57);
                page.MarginBottom(57);
                page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Times New Roman"));

                page.Content().Column(column =>
                {
                    column.Spacing(0);

                    // Order number
                    column.Item().AlignCenter().Text(text =>
                    {
                        text.Span("ƏMR № ").Bold();
                        text.Span($"{orderNumber:D3}").Bold();
                    });

                    column.Item().PaddingTop(30);

                    // Address and Date
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Text(companyAddress).Bold();
                        row.RelativeItem().AlignRight().Text($"{createdDate:dd} {GetMonthNameAz(createdDate.Month)} {createdDate:yyyy}-{createdYearSuffix} il").Bold();
                    });

                    column.Item().PaddingTop(35);

                    // Title
                    column.Item().AlignCenter().Text(text =>
                    {
                        text.Span($"{currentEmploymentTypeText} ştatdan {newEmploymentTypeText} vahidinə keçid haqqında")
                            .Bold();
                    });

                    column.Item().PaddingTop(30);

                    // Main paragraph
                    column.Item().Text(text =>
                    {
                        text.DefaultTextStyle(s => s.LineHeight(1.8f));
                        text.Justify();

                        text.Span($"{jobName.ToLower(new CultureInfo("az-Latn-AZ"))}{GetPossessiveSuffix(jobName)} ");
                        text.Span($"{fullName} {genderSuffixNun}").Bold().Underline();
                        text.Span(" razılıq bildirən ərizəsinə əsasən iş rejiminin dəyişdirilməsi ilə əlaqədar olaraq bağlanmış əmək müqaviləsinə ");
                        text.Span("Azərbaycan Respublikasının Əmək Məcəlləsinin 43-cü maddəsinin").Bold();
                        text.Span(" tələblərinə uyğun olaraq edilmiş dəyişikliklər əsasında.");
                    });

                    column.Item().PaddingTop(30);

                    // ƏMR EDİRƏM
                    column.Item().AlignCenter().Text("ƏMR EDİRƏM:").Bold().FontSize(13);

                    column.Item().PaddingTop(20);

                    // Paragraph 1
                    column.Item().Text(text =>
                    {
                        text.DefaultTextStyle(s => s.LineHeight(1.8f));
                        text.Justify();

                        text.Span("1. ").Bold();
                        text.Span($"{fullName} {genderParticle}").Bold().Underline();
                        text.Span($" – {jobName.ToLower(new CultureInfo("az-Latn-AZ"))}{GetPossessiveSuffix(jobName)} ");
                        text.Span($"{createdDate:dd.MM.yyyy}").Underline();
                        text.Span($"-{createdYearSuffix} il tarixindən etibarən {currentEmploymentTypeText} ştat üzrə tutduğu vəzifədən {newEmploymentTypeText} vahidinə keçirilsin.");
                    });

                    column.Item().PaddingTop(12);

                    // Paragraph 2
                    column.Item().Text(text =>
                    {
                        text.DefaultTextStyle(s => s.LineHeight(1.8f));
                        text.Justify();

                        text.Span("2. ").Bold();
                        text.Span($"Əmrin icrasına nəzarət {supervisorJobName.ToLower(new CultureInfo("az-Latn-AZ"))}{GetPossessiveSuffix(supervisorJobName)} ");
                        text.Span($"{supervisorFullName} {supervisorGenderParticle}na").Bold().Underline();
                        text.Span(" tapşırılsın.");
                    });

                    column.Item().PaddingTop(35);

                    // Əsas
                    column.Item().Text(text =>
                    {
                        text.DefaultTextStyle(s => s.LineHeight(1.8f));
                        text.Span("Əsas: ").Bold();
                        text.Span($"{fullName} {genderParticle}").Bold().Underline();
                        text.Span($" müraciət ərizəsi ({supervisorFullName} {supervisorGenderSuffixNun} dərkənarı ilə).");
                    });

                    column.Item().PaddingTop(70);

                    // Company name
                    column.Item().Text($"{companyName}-nin").Bold();

                    column.Item().PaddingTop(35);

                    // Signature
                    column.Item().Row(row =>
                    {
                        row.AutoItem().Text("Direktor").Bold();
                        row.RelativeItem().AlignRight().Text(directorName).Bold();
                    });
                });
            });
        }).GeneratePdf(stream);

        stream.Position = 0;
        return (stream, order.OrderNumber);
    }

    private static string GetEmploymentTypeText(EmploymentType employmentType)
    {
        return employmentType switch
        {
            EmploymentType.FullTime => "tam ştat",
            EmploymentType.HalfTime => "0,5 (yarım)",
            _ => "tam ştat"
        };
    }

    private static string GetMonthNameAz(int month)
    {
        return month switch
        {
            1 => "Yanvar",
            2 => "Fevral",
            3 => "Mart",
            4 => "Aprel",
            5 => "May",
            6 => "İyun",
            7 => "İyul",
            8 => "Avqust",
            9 => "Sentyabr",
            10 => "Oktyabr",
            11 => "Noyabr",
            12 => "Dekabr",
            _ => ""
        };
    }

    /// <summary>
    /// Azərbaycan dilində mənsubiyyət şəkilçisi qaytarır (müdiri, rəisi, direktoru və s.)
    /// </summary>
    private static string GetPossessiveSuffix(string word)
    {
        if (string.IsNullOrEmpty(word))
            return "i";

        // Son saiti tap
        var lowerWord = word.ToLower(new CultureInfo("az-Latn-AZ"));
        char lastVowel = ' ';

        for (int i = lowerWord.Length - 1; i >= 0; i--)
        {
            char c = lowerWord[i];
            if ("aıouəeiöü".Contains(c))
            {
                lastVowel = c;
                break;
            }
        }

        return lastVowel switch
        {
            'a' or 'ı' => "ı",
            'e' or 'ə' or 'i' => "i",
            'o' or 'u' => "u",
            'ö' or 'ü' => "ü",
            _ => "i"
        };
    }

    /// <summary>
    /// Azərbaycan dilində il üçün düzgün şəkilçi qaytarır (2025-ci, 2024-cü və s.)
    /// </summary>
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
            6 or 9 or 0 => "cı",
            _ => "ci"
        };
    }
}