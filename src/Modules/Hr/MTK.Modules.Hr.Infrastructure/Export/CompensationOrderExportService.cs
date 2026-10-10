using MTK.Modules.Hr.Application.Abstractions.Organization;
using MTK.Modules.Hr.Domain.CompensationOrders;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.JobApplications;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MTK.Modules.Hr.Infrastructure.Export;

internal sealed class CompensationOrderExportService
{
    private readonly IOrganizationInfo _organization;

    private readonly ICompensationOrderRepository _repository;

    public CompensationOrderExportService(ICompensationOrderRepository repository,
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
            ?? throw new InvalidOperationException($"CompensationOrder {orderId} not found.");

        var stream = new MemoryStream();

        // Data extraction
        var employee = order.Employee!;
        var fullName = $"{employee.Name} {employee.Surname} {employee.FathersName}";
        var gender = employee.Gender == Gender.Male ? "oğlu" : "qızı";
        var companyName = _organization.Name ?? "";
        var directorName = _organization.Director ?? "";
        var companyAddress = _organization.Address ?? "Bakı şəhəri";
        var position = employee.Job?.Name ?? "";
        var createdDate = order.CreatedAt.DateTime;
        var yearSuffix = GetYearSuffix(createdDate.Year);

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

                    // 2. ƏMR № (center, bold)
                    col.Item().PaddingTop(20).AlignCenter().Text(text =>
                    {
                        text.Span($"ƏMR № {order.OrderNumber:D4}").Bold();
                    });

                    col.Item().PaddingTop(25);

                    // 3. Location + Date
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text(companyAddress).Bold();
                        row.AutoItem().Text($"\"{createdDate.Day:D2}\" {GetMonthName(createdDate.Month)} {createdDate.Year}-{yearSuffix} il").Bold();
                    });

                    col.Item().PaddingTop(25);

                    // 4. Title (bold)
                    col.Item().Text(text =>
                    {
                        text.Span("\"Məzuniyyətə görə kompensasiyasının ödənilməsi barədə\"").Bold();
                    });

                    col.Item().PaddingTop(15);

                    // 5. Body paragraph
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

                            text.Span("cəmiyyətin ");

                            if (!string.IsNullOrWhiteSpace(position))
                            {
                                var positionLower = position.ToLowerInvariant();
                                text.Span(positionLower + GetDativeSuffix(positionLower)).Underline();
                            }
                            else
                            {
                                text.Span("_____________________________");
                            }

                            text.Span(", Azərbaycan Respublikasının Əmək Məcəlləsinin 135-ci maddəsinin 2-ci bəndinə əsasən ");

                            text.Span(order.WorkYearStart.HasValue && order.WorkYearEnd.HasValue
                                ? $"{order.WorkYearStart.Value:dd.MM.yyyy}-{GetYearSuffix(order.WorkYearStart.Value.Year)} ildən {order.WorkYearEnd.Value:dd.MM.yyyy}-{GetYearSuffix(order.WorkYearEnd.Value.Year)} ilədək "
                                : "__.__.____-cı ildən __.__.____-ci ilədək ");

                            text.Span("iş ili üçün istifadə etmədiyi əmək məzuniyyətinə görə ");
                            text.Span($"{order.CompensatedDays}").Underline();
                            text.Span($" ({NumberToWordsAz(order.CompensatedDays)})");
                            text.Span(" təqvim gününə kompensasiya ödənilsin.");
                        });
                    });

                    col.Item().PaddingTop(25);

                    // 6. Əsas
                    col.Item().Text("Əsas: İşçinin razılıq ərizəsi.");

                    col.Item().PaddingTop(35);

                    // 7. Company name
                    col.Item().Text(text =>
                    {
                        text.Span(companyName).Bold();
                    });

                    col.Item().PaddingTop(25);

                    // 8. Director
                    col.Item().Row(row =>
                    {
                        row.AutoItem().Text("Direktoru");
                        row.AutoItem().PaddingLeft(40).Text(directorLine);
                    });

                    col.Item().PaddingTop(50);

                    // 9. Employee signature
                    col.Item().Row(row =>
                    {
                        row.AutoItem().Text("İşçi (tanış oldum)");
                        row.RelativeItem().PaddingLeft(20).BorderBottom(1f).BorderColor(Colors.Black);
                        row.AutoItem().PaddingLeft(10).Text($"{fullName} {gender}");
                    });
                    col.Item().PaddingTop(2).PaddingLeft(135).Text("(imza)").FontSize(10);
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

    /// <summary>
    /// Azərbaycan dilində yönlük hal şəkilçisini müəyyən edir.
    /// Formal sənədlərdə həmişə vasitəçi sait + -na/-nə istifadə olunur.
    /// </summary>
    private static string GetDativeSuffix(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
            return "nə";

        var lastChar = char.ToLowerInvariant(word[^1]);

        // İncə saitlər
        char[] thinVowels = ['ə', 'e', 'i', 'ö', 'ü'];
        // Qalın saitlər
        char[] thickVowels = ['a', 'ı', 'o', 'u'];

        // Son simvol sait olarsa, vasitəçi "s" + sait uyğunluğu + na/nə
        if (thinVowels.Contains(lastChar))
        {
            return lastChar switch
            {
                'ə' => "sinə",
                'e' => "sinə",
                'i' => "sinə",
                'ö' => "sünə",
                'ü' => "sünə",  // sürücü → sürücüsünə
                _ => "sinə"
            };
        }
        if (thickVowels.Contains(lastChar))
        {
            return lastChar switch
            {
                'a' => "sına",
                'ı' => "sına",  // qapı → qapısına
                'o' => "suna",
                'u' => "suna",
                _ => "sına"
            };
        }

        // Son simvol samit olarsa, sözün son saitinə bax
        char lastVowel = '\0';
        for (int i = word.Length - 1; i >= 0; i--)
        {
            var ch = char.ToLowerInvariant(word[i]);
            if (thinVowels.Contains(ch) || thickVowels.Contains(ch))
            {
                lastVowel = ch;
                break;
            }
        }

        // Vasitəçi sait + -na/-nə (formal sənədlər üçün)
        return lastVowel switch
        {
            'a' or 'ı' => "ına",    // kitab → kitabına
            'o' or 'u' => "una",    // direktor → direktoruna
            'e' or 'i' => "inə",    // müdir → müdirinə, menecer → menecerinə
            'ö' or 'ü' => "ünə",    // söz → sözünə
            'ə' => "inə",             // dəftər → dəftərə (xüsusi hal, -nə deyil)
            _ => "nə"
        };
    }
}