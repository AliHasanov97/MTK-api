using MTK.Modules.Hr.Application.Abstractions.Organization;
using MTK.Modules.Hr.Domain.EmploymentOrders;
using MTK.Modules.Hr.Domain.JobApplications;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MTK.Modules.Hr.Infrastructure.Export;

internal sealed class EmploymentOrderExportService
{
    private readonly IOrganizationInfo _organization;

    private readonly IEmploymentOrderRepository _repository;

    public EmploymentOrderExportService(IEmploymentOrderRepository repository,
        IOrganizationInfo organization)
    {
        _organization = organization;
        _repository = repository;
    }

    public async Task<(MemoryStream Stream, int Index)> ExportToPdfAsync(
        Guid employmentOrderId,
        CancellationToken cancellationToken = default)
    {
        var eo = await _repository.GetByIdDefaultAsync(employmentOrderId, cancellationToken)
            ?? throw new InvalidOperationException($"EmploymentOrder {employmentOrderId} tapılmadı.");

        var ja           = eo.JobApplication!;
        var companyName  = _organization.Name     ?? "";
        var directorName = _organization.Director ?? "";
        var location     = _organization.Address  ?? "";
        var fullName     = $"{ja.Name} {ja.Surname} {ja.FathersName}";
        var gender       = ja.Gender == Gender.Male ? "oğlu" : "qızı";
        var jobTitle     = ja.Job?.Name ?? "";
        var orderNumber  = eo.OrderNumber;

        var startDateStr  = FormatDate(eo.StartDate);
        var startSuffix   = GetYearSuffix(eo.StartDate.Year);
        var endDateStr    = FormatDate(eo.EndDate);
        var endSuffix     = GetYearSuffix(eo.EndDate.Year);
        var jaDateStr     = FormatDate(ja.StartDate);
        var jaSuffix      = GetYearSuffix(ja.StartDate.Year);

        var laborParentCode   = eo.LaborCodeCase?.Parent?.Code ?? eo.LaborCodeCase?.Code ?? "";
        var laborParentSuffix = int.TryParse(laborParentCode, out var pNum)
            ? GetNumberSuffix(pNum)
            : "ci";
        var laborCode = eo.LaborCodeCase?.Code ?? "";
        var laborName = eo.LaborCodeCase?.Name ?? "";

        var stream = new MemoryStream();

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.MarginLeft(85);
                page.MarginRight(55);
                page.MarginTop(57);
                page.MarginBottom(57);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Times New Roman"));

                page.Content().Column(col =>
                {
                    col.Spacing(0);

                    // 1. Şirkət adı
                    col.Item().AlignCenter().Text(text =>
                    {
                        text.Span(companyName).Bold();
                    });

                    col.Item().PaddingTop(10).AlignCenter().Text(text =>
                    {
                        text.Span("üzrə").Bold();
                    });

                    // 2. Əmr №
                    col.Item().PaddingTop(20).AlignCenter().Text(text =>
                    {
                        text.Span($"Əmr № {orderNumber:D4}").Bold();
                    });

                    col.Item().PaddingTop(25);

                    // 3. Yer + tarix
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text(text => text.Span(location).Bold());
                        row.AutoItem().Text($"{eo.StartDate:dd.MM.yyyy}-{startSuffix} il");
                    });

                    col.Item().PaddingTop(25);

                    // 4. Başlıq
                    col.Item().Text(text =>
                    {
                        text.Span("“İşçinin işə qəbul edilməsi barədə”").Bold();
                    });

                    col.Item().PaddingTop(15);

                    // 5. Əsas mətn
                    col.Item().Row(row =>
                    {
                        row.AutoItem().PaddingRight(10).Text(text =>
                        {
                            text.Span($"1)  {fullName} {gender}").Bold();
                        });

                        row.RelativeItem().Text(text =>
                        {
                            text.Justify();
                            text.DefaultTextStyle(s => s.LineHeight(1.6f));

                            text.Span($"{startDateStr}-{startSuffix} il").Bold().Underline();
                            text.Span(" tarixən etibarən əmək haqqı əmək müqaviləsinə uyğun ödənilməklə ");
                            text.Span("Azərbaycan Respublikasının Əmək Məcəlləsinin ");

                            text.Span($"{laborParentCode}-{laborParentSuffix} maddəsinin “{laborCode}” bəndinə əsasən")
                                .FontColor("#FF0000")
                                .Underline();

                            text.Span(" (");

                            text.Span(laborName)
                                .FontColor("#FF0000")
                                .Italic();

                            text.Span(") ");

                            text.Span($"{endDateStr}-{endSuffix} il").Bold().Underline();
                            text.Span(" tarixədək cəmiyyətə müddətli ");

                            text.Span(jobTitle.ToLowerInvariant()).Underline();
                            text.Span(" vəzifəsinə/peşəsinə işə qəbul edilsin.");
                        });
                    });

                    col.Item().PaddingTop(25);

                    // 6. Əsas
                    col.Item().Text(text =>
                    {
                        text.Span("Əsas: ").Bold();
                        text.Span("İşçinin ");
                        text.Span($"{jaDateStr}-{jaSuffix} il").Bold().Underline();
                        text.Span(" tarixli ərizəsi və əmək müqaviləsi.");
                    });

                    col.Item().PaddingTop(35);

                    // 7. Şirkət adı (bağlanış)
                    col.Item().Text(text =>
                    {
                        text.Span(companyName).Bold();
                    });

                    col.Item().PaddingTop(35);

                    // 8. Direktor
                    col.Item().Row(row =>
                    {
                        row.AutoItem().Text(text => text.Span("Direktoru:").Bold());
                        row.RelativeItem().AlignRight().Text(text => text.Span(directorName).Bold());
                    });

                    col.Item().PaddingTop(50);

                    // 9. İmza yeri
                    col.Item().AlignRight().Text("M.Y.");
                });
            });
        }).GeneratePdf(stream);

        stream.Position = 0;
        return (stream, orderNumber);
    }

    private static string FormatDate(DateTimeOffset date) => $"{date:dd.MM.yyyy}";

    private static string GetNumberSuffix(int number)
    {
        int last = number % 10;
        return last switch
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
            _ => "cu"
        };
    }

    private static string GetYearSuffix(int year)
    {
        int lastTwo = year % 100;
        int last = lastTwo % 10;

        if (lastTwo == 0) return "ci";

        return last switch
        {
            0 => "ci",
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
    }
}