using MTK.Modules.Hr.Application.Abstractions.Organization;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.JobApplications;
using MTK.Modules.Hr.Domain.VacationReturnOrders;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MTK.Modules.Hr.Infrastructure.Export;

internal sealed class VacationReturnOrderExportService
{
    private readonly IOrganizationInfo _organization;

    private readonly IVacationReturnOrderRepository _repository;

    public VacationReturnOrderExportService(IVacationReturnOrderRepository repository,
        IOrganizationInfo organization)
    {
        _organization = organization;
        _repository = repository;
    }

    public async Task<(MemoryStream Stream, int Index)> ExportToPdfAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await _repository.GetByIdWithDetailsAsync(orderId, cancellationToken)
            ?? throw new InvalidOperationException($"VacationReturnOrder {orderId} not found.");

        return GeneratePdf(order);
    }

    private (MemoryStream, int) GeneratePdf(VacationReturnOrder order)
    {
        var stream = new MemoryStream();
        var employee = order.Employee!;
        var fullName = $"{employee.Name} {employee.Surname} {employee.FathersName}";
        var gender = employee.Gender == Gender.Male ? "oğlu" : "qızı";
        var companyName = _organization.Name ?? "";
        var directorName = _organization.Director ?? "";
        var position = employee.Job?.Name ?? "";
        var createdDate = order.CreatedAt.DateTime;
        var yearSuffix = GetYearSuffix(createdDate.Year);

        var returnDate = order.ReturnDate.DateTime;
        var returnYearSuffix = GetYearSuffix(returnDate.Year);

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.MarginLeft(70);
                page.MarginRight(50);
                page.MarginTop(50);
                page.MarginBottom(50);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Times New Roman"));

                page.Content().Column(column =>
                {
                    column.Spacing(0);

                    // Company name (center, bold) + "üzrə"
                    column.Item().AlignCenter().Text(text =>
                    {
                        text.Span($"\"{companyName}\"").Bold();
                    });
                    column.Item().PaddingTop(3).AlignCenter().Text("üzrə");

                    column.Item().PaddingTop(25);

                    // ƏMR №____ (center, underlined)
                    column.Item().AlignCenter().Text(text =>
                    {
                        text.Span("ƏMR №").Underline();
                        text.Span($"{order.OrderNumber}").Underline();
                    });

                    column.Item().PaddingTop(20);

                    // Company address (left) and full date (right) - same line
                    var companyAddress = _organization.Address ?? "Bakı şəhəri";
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Text(companyAddress);
                        row.RelativeItem().AlignRight().Text($"{createdDate.Day:D2}.{createdDate.Month:D2}.{createdDate.Year}-{yearSuffix} il");
                    });

                    column.Item().PaddingTop(15);

                    // Title - centered
                    column.Item().AlignCenter().Text(text =>
                    {
                        text.Span("\"Əmək məzuniyyətindən geriçağırma barədə\"").Bold().Italic();
                    });

                    column.Item().PaddingTop(20);

                    // Paragraph 1
                    column.Item().Text(text =>
                    {
                        text.DefaultTextStyle(s => s.LineHeight(1.5f));
                        text.Justify();
                        text.Span("1. Cəmiyyətin ");
                        text.Span($" {position.ToLowerInvariant()} ").Underline();
                        text.Span(" vəzifəsində/peşəsində çalışan ");
                        text.Span($"{fullName} {gender}").Bold();
                        text.Span(", istehsalatda baş vermiş qəzanın nəticələrinin aradan qaldırılması məqsədi ilə Azərbaycan Respublikasının Əmək Məcəlləsinin 137-ci maddəsinin 2-ci hissəsinə əsasən işəgötürənin təşəbbüsü və işçini razılığı ilə ");
                        text.Span($"{returnDate.Day:D2}.{returnDate.Month:D2}.{returnDate.Year}-{returnYearSuffix}").Underline();
                        text.Span(" il tarixdən etibaran ");
                        text.Span($"{createdDate.Day:D2}.{createdDate.Month:D2}.{createdDate.Year}-{yearSuffix}").Underline();
                        text.Span(" il tarixli ");
                        text.Span($"{order.OrderNumber}").Underline();
                        text.Span("№-li əmrlə verilmiş əmək məzuniyyətindən geri çağırılsın.");
                    });

                    column.Item().PaddingTop(15);

                    // Paragraph 2
                    column.Item().Text(text =>
                    {
                        text.DefaultTextStyle(s => s.LineHeight(1.5f));
                        text.Justify();
                        text.Span("2. İşçiyə tərəflərin razılığı ilə işə başladığı gündən etibarən əmək haqqı hesablansın və işlədiyi məzuniyyət günlərinin əvəzində gələcəkdə ödənişsiz əlavə istirahət günləri (əvəzgün) verilsin.");
                    });

                    column.Item().PaddingTop(30);

                    // Əmrin əsası
                    column.Item().Text(text =>
                    {
                        text.DefaultTextStyle(s => s.LineHeight(1.5f));
                        text.Span("Əmrin əsası: ");
                        text.Span($"İstehsalatda baş vermiş {createdDate.Day:D2}.{createdDate.Month:D2}.{createdDate.Year}-{yearSuffix} il tarixli qəza və ");
                        text.Span($"{fullName} {gender}").Bold();
                        text.Span("nın razılıq ərizəsi.");
                    });

                    column.Item().PaddingTop(30);

                    // Company name again (genitive case)
                    column.Item().Text(text =>
                    {
                        text.Span($"\"{companyName}\"nin").Bold();
                    });

                    column.Item().PaddingTop(25);

                    // Direktoru line
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Text(text =>
                        {
                            text.Span("  Direktoru").Bold();
                        });
                        row.RelativeItem().AlignRight().Text(directorName);
                    });

                    column.Item().PaddingTop(60);

                    // Employee signature line
                    column.Item().Row(row =>
                    {
                        row.AutoItem().Text(text =>
                        {
                            text.Span("İşçi (əmrlə tanış oldum)").Bold();
                        });
                        row.AutoItem().Width(120).BorderBottom(1f).Text("");
                        row.AutoItem().PaddingLeft(5).Text(text =>
                        {
                            text.Span($"{fullName} {gender}").Bold();
                        });
                    });

                    column.Item().PaddingLeft(155).PaddingTop(2).Text("(imza)").FontSize(10);
                });
            });
        }).GeneratePdf(stream);

        stream.Position = 0;
        return (stream, order.OrderNumber);
    }

    #region Helper Methods

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

    #endregion
}
