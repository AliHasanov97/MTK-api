using MTK.Modules.Hr.Application.Abstractions.Organization;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.JobApplications;
using MTK.Modules.Hr.Domain.Warnings;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MTK.Modules.Hr.Infrastructure.Export;

internal sealed class WarningExportService
{
    private readonly IOrganizationInfo _organization;

    private readonly IWarningRepository _repository;

    public WarningExportService(IWarningRepository repository,
        IOrganizationInfo organization)
    {
        _organization = organization;
        _repository = repository;
    }

    public async Task<(MemoryStream Stream, int Index, DisciplinaryType DisciplinaryType)> ExportToPdfAsync(
        Guid warningId,
        CancellationToken cancellationToken = default)
    {
        var warning = await _repository.GetByIdForExportAsync(warningId, cancellationToken)
            ?? throw new InvalidOperationException($"Warning {warningId} not found.");

        var stream = new MemoryStream();

        // Data extraction
        var employee = warning.Employee!;
        var supervisor = warning.OrderExecutionSupervisor!;
        var fullName = $"{employee.Name} {employee.Surname} {employee.FathersName}";
        var gender = employee.Gender == Gender.Male ? "oğlu" : "qızı";
        var supervisorFullName = $"{supervisor.Name} {supervisor.Surname} {supervisor.FathersName}";
        var supervisorGender = supervisor.Gender == Gender.Male ? "oğlu" : "qızı";
        var companyName = _organization.Name;
        var companyAddress = _organization.Address;
        var directorName = _organization.Director;
        var jobName = employee.Job?.Name;
        var setDate = warning.SetDate;
        var createdDate = warning.CreatedAt;
        var setYearSuffix = GetYearSuffix(setDate.Year);
        var createdYearSuffix = GetYearSuffix(createdDate.Year);
        var orderNumber = warning.OrderNumber;
        var disciplinaryType = warning.DisciplinaryType;
        var disciplinaryActionTitle = GetDisciplinaryActionTitle(disciplinaryType);
        var disciplinaryActionText = GetDisciplinaryActionText(disciplinaryType);

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

                    column.Item().PaddingTop(30);

                    // Address and date
                    column.Item().Row(row =>
                    {
                        row.AutoItem().Text(text =>
                        {
                            text.Span(companyAddress).Bold().Italic();
                        });
                        row.RelativeItem().AlignRight().Text(text =>
                        {
                            text.Span($"{createdDate:dd}.{createdDate:MM}.{createdDate:yyyy}-{createdYearSuffix} il").Bold();
                        });
                    });

                    column.Item().PaddingTop(30);

                    // Title
                    column.Item().Text(text =>
                    {
                        text.Span($"\"Əmək intizamının pozulmasını nəzərə alaraq {disciplinaryActionTitle} verilməsi haqqında\"").Bold();
                    });

                    column.Item().PaddingTop(20);

                    // Main intro paragraph
                    column.Item().Text(text =>
                    {
                        text.DefaultTextStyle(s => s.LineHeight(1.8f));
                        text.Justify();
                        text.Span($"{companyName}-nin ");
                        text.Span($"\"{jobName?.ToLowerInvariant()}\"").Underline();
                        text.Span(" vəzifəsində çalışan ");
                        text.Span($"{fullName} {gender}").Underline();
                        text.Span($" {setDate:dd}.{setDate:MM}.{setDate:yyyy}-{setYearSuffix} il tarixdə şirkət daxili nizam-intizam qaydalarını kobud şəkildə pozmuşdur. Qeyd olunan halı nəzərə alaraq");
                    });

                    column.Item().PaddingTop(20);

                    // "ƏMR edirəm" header
                    column.Item().AlignCenter().Text("ƏMR edirəm").Bold();

                    column.Item().PaddingTop(20);

                    // Paragraph 1 - dynamic based on order type
                    column.Item().Text(text =>
                    {
                        text.DefaultTextStyle(s => s.LineHeight(1.8f));
                        text.Justify();
                        text.Span("1)").Bold();
                        text.Span(GetParagraph1LegalBasis(disciplinaryType));
                        text.Span($"{fullName} {gender}na").Underline();
                        text.Span(GetParagraph1Ending(disciplinaryType));
                    });

                    column.Item().PaddingTop(10);

                    // Paragraph 2 - dynamic based on order type
                    column.Item().Text(text =>
                    {
                        text.DefaultTextStyle(s => s.LineHeight(1.8f));
                        text.Justify();
                        text.Span("2)").Bold();
                        text.Span(GetParagraph2Text(disciplinaryType));
                    });

                    column.Item().PaddingTop(10);

                    // Paragraph 3
                    column.Item().Text(text =>
                    {
                        text.DefaultTextStyle(s => s.LineHeight(1.8f));
                        text.Justify();
                        text.Span("3)").Bold();
                        text.Span(" İşçi əmrin mətni ilə tanış edilsin.");
                    });

                    column.Item().PaddingTop(10);

                    // Paragraph 4
                    column.Item().Text(text =>
                    {
                        text.DefaultTextStyle(s => s.LineHeight(1.8f));
                        text.Justify();
                        text.Span("4)").Bold();
                        text.Span($" {GetDisciplinaryActionTitleCapitalized(disciplinaryType)} barədə əmrin çıxarışı ");
                        text.Span($"{fullName} {gender}{GetPossessiveSuffixNin(gender)}").Underline();
                        text.Span(" şəxsi işinə tikilsin.");
                    });

                    column.Item().PaddingTop(10);

                    // Paragraph 5 - supervisor
                    column.Item().Text(text =>
                    {
                        text.DefaultTextStyle(s => s.LineHeight(1.8f));
                        text.Justify();
                        text.Span("5)").Bold();
                        text.Span(" Əmrin icrasına nəzarəti ");
                        text.Span($"{supervisorFullName} {supervisorGender}na").Underline();
                        text.Span(" tapşırıram.");
                    });

                    column.Item().PaddingTop(20);

                    // Basis
                    column.Item().Text(text =>
                    {
                        text.Span("Əsas:").Bold();
                        text.Span(" Məsul şəxsin təqdimatı və işçinin izahatı");
                    });

                    column.Item().PaddingTop(30);

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

                    // Employee signature section
                    column.Item().Row(row =>
                    {
                        row.AutoItem().Text(text =>
                        {
                            text.Span("İşçi (tanış oldum)").Underline();
                        });
                        row.RelativeItem().AlignRight().Text(text =>
                        {
                            text.Span($"{fullName} {gender}").Underline();
                        });
                    });
                });
            });
        }).GeneratePdf(stream);

        stream.Position = 0;
        return (stream, warning.OrderNumber, warning.DisciplinaryType);
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
    /// Returns the correct possessive suffix "nın/nun" based on vowel harmony for gender.
    /// "qızı" (daughter) → "nın", "oğlu" (son) → "nun"
    /// </summary>
    private static string GetPossessiveSuffixNin(string gender)
    {
        return gender == "oğlu" ? "nun" : "nın";
    }

    /// <summary>
    /// Returns the disciplinary action title based on disciplinary type (lowercase).
    /// </summary>
    private static string GetDisciplinaryActionTitle(DisciplinaryType disciplinaryType)
    {
        return disciplinaryType switch
        {
            DisciplinaryType.Warning => "xəbərdarlıq",
            DisciplinaryType.Reprimand => "töhmət",
            DisciplinaryType.SevereReprimand => "şiddətli töhmət",
            _ => "xəbərdarlıq"
        };
    }

    /// <summary>
    /// Returns the disciplinary action title based on disciplinary type (capitalized).
    /// </summary>
    private static string GetDisciplinaryActionTitleCapitalized(DisciplinaryType disciplinaryType)
    {
        return disciplinaryType switch
        {
            DisciplinaryType.Warning => "Xəbərdarlıq",
            DisciplinaryType.Reprimand => "Töhmət",
            DisciplinaryType.SevereReprimand => "Şiddətli töhmət",
            _ => "Xəbərdarlıq"
        };
    }

    /// <summary>
    /// Returns the disciplinary action text for paragraph 1.
    /// </summary>
    private static string GetDisciplinaryActionText(DisciplinaryType disciplinaryType)
    {
        return disciplinaryType switch
        {
            DisciplinaryType.Warning => "yazılı xəbərdarlıq",
            DisciplinaryType.Reprimand => "töhmət",
            DisciplinaryType.SevereReprimand => "şiddətli töhmət",
            _ => "yazılı xəbərdarlıq"
        };
    }

    /// <summary>
    /// Returns the legal basis text for paragraph 1 based on disciplinary type.
    /// </summary>
    private static string GetParagraph1LegalBasis(DisciplinaryType disciplinaryType)
    {
        return disciplinaryType switch
        {
            DisciplinaryType.Warning => " Azərbaycan Respublikasının Əmək Məcəlləsinin 186-cı maddəsinin 3-cü hissəsinə əsasən ",
            DisciplinaryType.Reprimand => " Azərbaycan Respublikasının Əmək Məcəlləsinin 186-cı maddəsinin 2-ci hissəsinin \"a\" bəndinə əsasən ",
            DisciplinaryType.SevereReprimand => " Azərbaycan Respublikasının Əmək Məcəlləsinin 186-cı maddəsinin 2-ci hissəsinin \"b\" bəndinə əsasən ",
            _ => " Azərbaycan Respublikasının Əmək Məcəlləsinin 186-cı maddəsinin 3-cü hissəsinə əsasən "
        };
    }

    /// <summary>
    /// Returns the ending text for paragraph 1 based on disciplinary type.
    /// </summary>
    private static string GetParagraph1Ending(DisciplinaryType disciplinaryType)
    {
        return disciplinaryType switch
        {
            DisciplinaryType.Warning => " yazılı xəbərdarlıq elan edilsin.",
            DisciplinaryType.Reprimand => " töhmət elan edilsin.",
            DisciplinaryType.SevereReprimand => " sonuncu xəbərdarlıqla şiddətli töhmət elan edilsin.",
            _ => " yazılı xəbərdarlıq elan edilsin."
        };
    }

    /// <summary>
    /// Returns the text for paragraph 2 based on disciplinary type.
    /// </summary>
    private static string GetParagraph2Text(DisciplinaryType disciplinaryType)
    {
        return disciplinaryType switch
        {
            DisciplinaryType.Warning => " İşçinin nəzərinə çatdırılsın ki, gələcəkdə belə hallar təkrar baş verdikdə qanunvericiliyə əsasən ona qarşı daha ciddi tədbirlər görülsün.",
            DisciplinaryType.Reprimand => " İşçinin nəzərinə çatdırılsın ki, gələcəkdə belə hallar təkrar baş verdikdə qanunvericiliyə əsasən ona qarşı daha ciddi tədbirlər görüləcək.",
            DisciplinaryType.SevereReprimand => " İşçinin nəzərinə çatdırılsın ki, gələcəkdə belə hallar təkrar baş verdikdə Azərbaycan Respublikasının Əmək Məcəlləsinin 70-ci maddəsinin \"ç\" maddəsinə əsasən (işçi özünün əmək funksiyasını və ya əmək müqaviləsi üzrə öhdəliklərini yerinə yetirmədikdə) tutduğu vəzifədən azad olunacaq.",
            _ => " İşçinin nəzərinə çatdırılsın ki, gələcəkdə belə hallar təkrar baş verdikdə qanunvericiliyə əsasən ona qarşı daha ciddi tədbirlər görülsün."
        };
    }
}
