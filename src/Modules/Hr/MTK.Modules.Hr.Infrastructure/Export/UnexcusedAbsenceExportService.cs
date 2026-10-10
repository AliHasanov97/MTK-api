using MTK.Modules.Hr.Application.Abstractions.Organization;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.JobApplications;
using MTK.Modules.Hr.Domain.UnexcusedAbsences;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MTK.Modules.Hr.Infrastructure.Export;

internal sealed class UnexcusedAbsenceExportService
{
    private readonly IOrganizationInfo _organization;

    private readonly IUnexcusedAbsenceRepository _repository;

    public UnexcusedAbsenceExportService(IUnexcusedAbsenceRepository repository,
        IOrganizationInfo organization)
    {
        _organization = organization;
        _repository = repository;
    }

    public async Task<(MemoryStream Stream, int Index)> ExportToPdfAsync(
        Guid unexcusedAbsenceId,
        CancellationToken cancellationToken = default)
    {
        var absence = await _repository.GetByIdDefaultAsync(unexcusedAbsenceId, cancellationToken)
            ?? throw new InvalidOperationException($"UnexcusedAbsence {unexcusedAbsenceId} not found.");

        var stream = new MemoryStream();

        // Data extraction
        var employee = absence.Employee!;
        var fullName = $"{employee.Name} {employee.Surname} {employee.FathersName}";
        var gender = employee.Gender == Gender.Male ? "oğlu" : "qızı";
        var companyName = _organization.Name;
        var companyAddress = _organization.Address;
        var directorName = _organization.Director;
        var jobName = employee.Job?.Name;
        var setDate = absence.SetDate;
        var yearSuffix = GetYearSuffix(setDate.Year);
        var orderNumber = absence.OrderNumber;

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

                    // Header - Company name
                    column.Item().AlignCenter().Text(text =>
                    {
                        text.Span(companyName).Bold();
                    });

                    column.Item().PaddingTop(40);

                    // "üzrə" - center
                    column.Item().AlignCenter().Text("üzrə").Bold();

                    column.Item().PaddingTop(20);

                    // Order number
                    column.Item().AlignCenter().Text(text =>
                    {
                        text.Span("ƏMR № ").Bold();
                        text.Span($"{orderNumber:D4}").Underline();
                    });

                    column.Item().PaddingTop(60);

                    // Address and date
                    column.Item().Row(row =>
                    {
                        row.AutoItem().Text(companyAddress).Bold();
                        row.RelativeItem().AlignRight().Text(text =>
                        {
                            text.Span($"{setDate:dd}.{setDate:MM}.{setDate:yyyy}-{yearSuffix} il").Underline().Bold();
                        });
                    });

                    column.Item().PaddingTop(40);

                    // Title (quoted)
                    column.Item().Text(text =>
                    {
                        text.Span("\"İşə gəlməmək barədə\"").Bold();
                    });

                    column.Item().PaddingTop(30);

                    // Main content - paragraph 1
                    column.Item().Text(text =>
                    {
                        text.DefaultTextStyle(s => s.LineHeight(1.8f));
                        text.Justify();
                        text.Span("1. Cəmiyyətdə ");
                        text.Span($"\"{jobName?.ToLowerInvariant()}\"").Underline();
                        text.Span(" vəzifəsində çalışan ");
                        text.Span($"{fullName} {gender}").Underline();
                        text.Span($" {setDate:dd}.{setDate:MM}.{setDate:yyyy}-{yearSuffix} il tarixdə heç bir üzürlü səbəb bildirmədən tam iş günü ərzində işə (iş yerinə) gəlmədiyinə görə həmin iş günü üçün \"iş vaxtının uçotu tabelində\" işə gəlməmək kimi qeydə alınsın.");
                    });

                    column.Item().PaddingTop(15);

                    // Main content - paragraph 2
                    column.Item().Text(text =>
                    {
                        text.DefaultTextStyle(s => s.LineHeight(1.8f));
                        text.Justify();
                        text.Span("2. İnsan resursları şöbəsi məsələni araşdırsın və Qanunvericiliyin tələblərinə əsasən müvafiq tədbirlər görsün.");
                    });

                    column.Item().PaddingTop(60);

                    // Company name at bottom
                    column.Item().Text(text =>
                    {
                        text.Span(companyName).Bold();
                    });

                    column.Item().PaddingTop(80);

                    // Signature section
                    column.Item().Row(row =>
                    {
                        row.AutoItem().Text("Direktoru").Bold();

                        row.RelativeItem().AlignRight().Text(directorName).Bold();
                    });
                });
            });
        }).GeneratePdf(stream);

        stream.Position = 0;
        return (stream, absence.OrderNumber);
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
}
