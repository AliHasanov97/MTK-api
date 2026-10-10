using MTK.Modules.Hr.Application.Abstractions.Organization;
using MTK.Modules.Hr.Domain.JobApplications;
using MTK.Modules.Hr.Domain.SalaryDeductions;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MTK.Modules.Hr.Infrastructure.Export;

internal sealed class SalaryDeductionExportService
{
    private readonly IOrganizationInfo _organization;

    private readonly ISalaryDeductionRepository _repository;

    public SalaryDeductionExportService(ISalaryDeductionRepository repository,
        IOrganizationInfo organization)
    {
        _organization = organization;
        _repository = repository;
    }

    public async Task<(MemoryStream Stream, int Index)> ExportToPdfAsync(
        Guid salaryDeductionId,
        CancellationToken cancellationToken = default)
    {
        var salaryDeduction = await _repository.GetByIdForExportAsync(salaryDeductionId, cancellationToken)
            ?? throw new InvalidOperationException($"SalaryDeduction {salaryDeductionId} not found.");

        var stream = new MemoryStream();

        // Data extraction
        var employee = salaryDeduction.Employee!;
        var fullName = $"{employee.Name} {employee.Surname} {employee.FathersName}";
        var gender = employee.Gender == Gender.Male ? "oğlu" : "qızı";
        var genderSuffixNun = employee.Gender == Gender.Male ? "oğlunun" : "qızının";
        var jobName = employee.Job?.Name;
        var companyName = _organization.Name;
        var directorName = _organization.Director;

        var district = salaryDeduction.District;
        var judgementNo = salaryDeduction.JudgementNo;
        var judgementDate = salaryDeduction.JudgementDate;
        var startDate = salaryDeduction.StartDate;
        var percentageSalary = salaryDeduction.PercentageSalary;
        var stateFee = salaryDeduction.StateFee;
        var creditor = salaryDeduction.Creditor;
        var debt = salaryDeduction.Debt;
        var orderNumber = salaryDeduction.OrderNumber;
        var createdDate = salaryDeduction.CreatedAt;

        var stateFeeWords = NumberToMoneyWordsAz(stateFee);
        var debtWords = NumberToMoneyWordsAz(debt);
        var executionFee = debt;
        var executionFeeWords = NumberToMoneyWordsAz(executionFee);

        // PDF generation with QuestPDF
        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.MarginLeft(70);
                page.MarginRight(70);
                page.MarginTop(50);
                page.MarginBottom(50);
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

                    column.Item().PaddingTop(15);

                    // "üzrə" - center
                    column.Item().AlignCenter().Text("üzrə").Bold();

                    column.Item().PaddingTop(15);

                    // Order number
                    column.Item().AlignCenter().Text(text =>
                    {
                        text.Span("ƏMR №").Bold();
                        text.Span($"{orderNumber:D4}").Underline().Bold();
                    });

                    column.Item().PaddingTop(15);

                    // City and date row
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Text("Bakı şəhəri").Bold();
                        row.RelativeItem().AlignRight().Text($"{createdDate:dd}.{createdDate:MM}.{createdDate:yyyy}").Bold();
                    });

                    column.Item().PaddingTop(15);

                    // Title
                    column.Item().Text(text =>
                    {
                        text.Span($"{createdDate:yyyy}").Bold();
                        text.Span("-").Bold();
                        text.Span(GetYearSuffix(createdDate.Year)).Bold();
                        text.Span(" il  ").Bold();
                        text.Span("\"Əmək haqqından tutulma barədə\"").Bold();
                    });

                    column.Item().PaddingTop(20);

                    // Main content with left label and right text
                    column.Item().Row(row =>
                    {
                        // Left column - employee name label
                        row.ConstantItem(180).AlignTop().Text(text =>
                        {
                            text.DefaultTextStyle(s => s.LineHeight(1.6f));
                            text.Span("1. ").Bold();
                            text.Span($"{fullName} {gender}").Bold();
                        });

                        // Right column - main content
                        row.RelativeItem().Text(text =>
                        {
                            text.DefaultTextStyle(s => s.LineHeight(1.6f));
                            text.Justify();
                            text.Span("Cəmiyyətin ");
                            text.Span($"{jobName?.ToLowerInvariant()}").Underline();
                            text.Span($"-{GetGenitiveSuffix(jobName)} aylıq əmək haqqından, Azərbaycan Respublikasının Əmək məcəlləsinin 175-ci maddəsinin 2-ci hissəsinin \"b\" bəndini ");
                            text.Span("(qanunvericilikdə nəzərdə tutulan icra sənədləri üzrə müəyyən edilmiş məbləğ)").Italic();
                            text.Span(" rəhbər tutaraq, ");
                            text.Span($"{district}").Underline();
                            text.Span(" rayon məhkəməsinin ");
                            text.Span($"{judgementDate:dd}.{judgementDate:MM}.{judgementDate:yyyy}").Underline();
                            text.Span("-cı il tarixli ");
                            text.Span($"{judgementNo}").Underline();
                            text.Span(" nömrəli qərarına əsasən ");
                            text.Span($"{startDate:dd}.{startDate:MM}.{startDate:yyyy}").Underline();
                            text.Span("-cı il tarixdən etibarən aylıq qazancın ");
                            text.Span($"{percentageSalary}").Underline();
                            text.Span("%-i məbləği və ");
                            text.Span($"{stateFee:N2}").Underline();
                            text.Span($" ({stateFeeWords})");
                            text.Span(" AZN dövlət rüsumu tutularaq ");
                            text.Span($"{creditor}").Underline();
                            text.Span($"-{GetGenitiveSuffix(creditor)} bank hesabına, həmçinin \"İcra Haqqında\" A.R. Qanununun 78-ci maddəsinə əsasən ");
                            text.Span($"{executionFee:N2}").Underline();
                            text.Span($" ({executionFeeWords})");
                            text.Span(" AZN borcun 7 % icra ödənişi ");
                            text.Span($"{district}").Underline();
                            text.Span(" rayon İcra şöbəsinin depozit hesabına köçürülməsi təmin edilsin.");
                            text.EmptyLine();
                            text.Span("2) Əmrin icrası mühasibatlığa tapşırılsın.");
                            text.EmptyLine();
                            text.Span("3) İşçi əmrlə tanış edilsin.");
                        });
                    });

                    column.Item().PaddingTop(25);

                    // Əsas section
                    column.Item().Text(text =>
                    {
                        text.DefaultTextStyle(s => s.LineHeight(1.6f));
                        text.Span("Əsas: ").Bold();
                        text.Span($"{district}").Underline();
                        text.Span(" rayon məhkəməsinin ");
                        text.Span($"{judgementDate:dd}.{judgementDate:MM}.{judgementDate:yyyy}").Underline();
                        text.Span("-cı il tarixli ");
                        text.Span($"{judgementNo}").Underline();
                        text.Span(" nömrəli qərarı");
                    });

                    column.Item().PaddingTop(30);

                    // Company name at bottom
                    column.Item().Text(text =>
                    {
                        text.Span($"{companyName}-nin ").Bold();
                    });

                    column.Item().PaddingTop(25);

                    // Director signature section
                    column.Item().Row(row =>
                    {
                        row.AutoItem().Text("Direktoru").Bold();
                        row.RelativeItem().AlignRight().Text($"{directorName}").Bold();
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
        return (stream, salaryDeduction.OrderNumber);
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
            words += thousands == 1 ? "min " : NumberToWordsAz(thousands) + " min ";
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

    /// <summary>
    /// Converts a decimal amount to Azerbaijani money words (manat and qəpik).
    /// </summary>
    private static string NumberToMoneyWordsAz(decimal amount)
    {
        var manat = (int)amount;
        var qepik = (int)Math.Round((amount - manat) * 100);

        var result = "";

        if (manat > 0)
        {
            result = NumberToWordsAz(manat) + " manat";
        }

        if (qepik > 0)
        {
            if (!string.IsNullOrEmpty(result))
                result += " ";
            result += NumberToWordsAz(qepik) + " qəpik";
        }

        if (string.IsNullOrEmpty(result))
            result = "sıfır manat";

        return result;
    }

    /// <summary>
    /// Returns the correct Azerbaijani genitive suffix (-ın/-in/-un/-ün or -nın/-nin/-nun/-nün) based on vowel harmony.
    /// </summary>
    private static string GetGenitiveSuffix(string? word)
    {
        if (string.IsNullOrWhiteSpace(word))
            return "ın";

        var lastVowel = word.ToLowerInvariant()
            .LastOrDefault(c => "aeəiıoöuü".Contains(c));

        var endsWithVowel = "aeəiıoöuü".Contains(word.ToLowerInvariant().Last());
        var prefix = endsWithVowel ? "n" : "";

        return lastVowel switch
        {
            'a' or 'ı' => prefix + "ın",
            'e' or 'ə' or 'i' => prefix + "in",
            'o' or 'u' => prefix + "un",
            'ö' or 'ü' => prefix + "ün",
            _ => prefix + "ın"
        };
    }
}
