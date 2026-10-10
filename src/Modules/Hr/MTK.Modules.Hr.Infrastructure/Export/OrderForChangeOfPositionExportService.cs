using MTK.Modules.Hr.Application.Abstractions.Organization;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.JobApplications;
using MTK.Modules.Hr.Domain.OrdersForChangeOfPosition;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MTK.Modules.Hr.Infrastructure.Export;

internal sealed class OrderForChangeOfPositionExportService
{
    private readonly IOrganizationInfo _organization;

    private readonly IOrderForChangeOfPositionRepository _repository;

    public OrderForChangeOfPositionExportService(IOrderForChangeOfPositionRepository repository,
        IOrganizationInfo organization)
    {
        _organization = organization;
        _repository = repository;
    }

    public async Task<(MemoryStream Stream, int Index)> ExportToPdfAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await _repository.GetByIdDefaultAsync(orderId, cancellationToken)
            ?? throw new InvalidOperationException($"OrderForChangeOfPosition {orderId} not found.");

        var stream = new MemoryStream();

        var application = order.ApplicationForChangeOfPosition!;
        var employee = application.Employee!;
        var fullName = $"{employee.Name} {employee.Surname} {employee.FathersName}";
        var gender = employee.Gender == Gender.Male ? "oğlu" : "qızı";
        var companyName = _organization.Name ?? "";
        var directorName = _organization.Director ?? "";
        var companyAddress = _organization.Address ?? "Bakı şəhəri";
        var newPosition = application.NewJob?.Name ?? "";
        var setDate = application.SetDate;
        var yearSuffix = GetYearSuffix(setDate.Year);

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

                    // 1. Company name (center, bold)
                    col.Item().AlignCenter().Text(text =>
                    {
                        text.Span(companyName).Bold();
                    });

                    col.Item().PaddingTop(10).AlignCenter().Text(text =>
                    {
                        text.Span("üzrə").Bold();
                    });

                    // 2. ƏMR № (center, bold) with order number
                    col.Item().PaddingTop(20).AlignCenter().Text(text =>
                    {
                        text.Span($"ƏMR № {order.OrderNumber:D4}").Bold();
                    });

                    col.Item().PaddingTop(10);

                    // 3. Right side: Company address and date
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(left =>
                        {
                            left.Item().Text(companyAddress).Bold();
                        });

                        row.RelativeItem().Column(right =>
                        {
                            right.Item().AlignRight().Text(text =>
                            {
                                text.Span($"{setDate.Day:D2}.{setDate.Month:D2}. {setDate.Year}-{yearSuffix} il");
                            });
                        });
                    });

                    col.Item().PaddingTop(20);

                    // 4. Bold title
                    col.Item().AlignCenter().Text(text =>
                    {
                        text.Span("\"Başqa işə keçirilmə barədə\"").Bold();
                    });

                    col.Item().PaddingTop(20);

                    // 5. Main text paragraph - 2 column layout
                    col.Item().Row(mainRow =>
                    {
                        // Left column: Employee name
                        mainRow.AutoItem().Width(150).Text(text =>
                        {
                            text.Span($"1) {fullName} {gender}").Bold();
                        });

                        // Right column: Main text
                        mainRow.RelativeItem().Text(text =>
                        {
                            text.DefaultTextStyle(s => s.LineHeight(1.5f));
                            text.Justify();
                            text.Span("cəmiyyətin ");
                            text.Span(setDate.ToString("dd.MM.yyyy")).Underline();
                            text.Span($"-{yearSuffix} il tarixdən etibarən əmək haqqı əmək müqaviləsinə uyğun ödənilməklə Azərbaycan Respublikasının Əmək Məcəlləsinin 59-cu maddəsinə əsasən ");
                            text.Span("(başqa işə keçirmə)").Italic();
                            text.Span(" ");
                            text.Span(newPosition.ToLowerInvariant()).Underline();
                            text.Span(" vəzifəsinə/peşəsinə keçirilsin. İşçi ilə tam maddi məsuliyyət haqqında müqavilə bağlanılsın.");
                        });
                    });

                    col.Item().PaddingTop(20);

                    // 6. Əsas: İşçinin razılıq ərizəsi
                    col.Item().Text(text =>
                    {
                        text.Span("Əsas:").Bold();
                        text.Span(" İşçinin razılıq ərizəsi");
                    });

                    col.Item().PaddingTop(40);

                    // 7. Bottom section: Company, Director, Employee
                    col.Item().Column(bottomCol =>
                    {
                        bottomCol.Spacing(0);

                        // Company name
                        bottomCol.Item().Text(text =>
                        {
                            text.Span($"{companyName}-nin").Bold();
                        });

                        bottomCol.Item().PaddingTop(10);

                        // Director signature line
                        bottomCol.Item().Row(dirRow =>
                        {
                            dirRow.AutoItem().Width(100).Text(text => text.Span("Direktoru").Bold());
                            dirRow.RelativeItem().AlignRight().Text(text =>
                            {
                                text.Span(directorName).Italic();
                            });
                        });

                        bottomCol.Item().PaddingTop(30);

                        // Employee signature line
                        bottomCol.Item().Row(empRow =>
                        {
                            empRow.AutoItem().Text(text => text.Span("İşçi (tanış oldum) ").Bold());
                            empRow.AutoItem().Width(150).BorderBottom(1f).BorderColor(Colors.Black);
                            empRow.RelativeItem().AlignRight().Column(nameCol =>
                            {
                                nameCol.Item().AlignRight().Text(text =>
                                {
                                    text.Span($"{fullName} {gender}").Italic();
                                });
                            });
                        });
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
}
