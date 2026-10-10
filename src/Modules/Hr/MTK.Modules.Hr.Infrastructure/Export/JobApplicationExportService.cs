using MTK.Modules.Hr.Application.Abstractions.Organization;
using MTK.Modules.Hr.Domain.JobApplications;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MTK.Modules.Hr.Infrastructure.Export;

internal sealed class JobApplicationExportService
{
    private readonly IOrganizationInfo _organization;

    private readonly IJobApplicationRepository _repository;

    public JobApplicationExportService(IJobApplicationRepository repository,
        IOrganizationInfo organization)
    {
        _organization = organization;
        _repository = repository;
    }

    public async Task<(MemoryStream Stream, int Index)> ExportToPdfAsync(
        Guid jobApplicationId,
        CancellationToken cancellationToken = default)
    {
        var ja = await _repository.GetByIdDefaultAsync(jobApplicationId, cancellationToken)
            ?? throw new InvalidOperationException($"JobApplication {jobApplicationId} not found.");

        var stream = new MemoryStream();

        var companyName   = _organization.Name     ?? "";
        var directorName  = _organization.Director ?? "";
        var fullName      = $"{ja.Name} {ja.Surname} {ja.FathersName}";
        var gender        = ja.Gender == Gender.Male ? "oğlu" : "qızı";

        var phoneDisplay  = string.IsNullOrWhiteSpace(ja.HomeTelephoneNumber)
            ? ja.Telephone
            : $"{ja.Telephone}  /  {ja.HomeTelephoneNumber}";

        var directorLine  = string.IsNullOrWhiteSpace(directorName)
            ? "____________________________"
            : directorName;

        var yearSuffix = GetYearSuffix(ja.StartDate.Year);

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);

                // Margins from original Word doc (twips / 20 = pt):
                // left=1701→85pt, right=836→42pt, top/bottom=1134→57pt
                page.MarginLeft(85);
                page.MarginRight(42);
                page.MarginTop(57);
                page.MarginBottom(57);
                page.PageColor(Colors.White);

                // Default: Times New Roman, 12pt, normal weight
                page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Times New Roman"));

                page.Content().Column(column =>
                {
                    column.Spacing(0);

                    // ── Addressee block ──────────────────────────────────────
                    // Indented ~193pt from left (3870 twips / 20), BOLD
                    column.Item().PaddingLeft(193).Column(addrCol =>
                    {
                        addrCol.Spacing(2);

                        addrCol.Item().Text(text =>
                        {
                            text.Span($"{companyName} direktoru {directorLine}").Bold();
                        });
                    });

                    column.Item().PaddingTop(10);

                    // ── Applicant info block (right half, matches screenshot layout) ──
                    column.Item().PaddingLeft(193).Column(appCol =>
                    {
                        appCol.Spacing(0);

                        // ── Field 1: address ──────────────────────────────────
                        // italic value, then underline, then "(ünvan)" label
                        appCol.Item().Text(ja.Address ?? "").Italic();
                        appCol.Item().PaddingTop(2).Height(14).BorderBottom(0.5f).BorderColor(Colors.Black);
                        appCol.Item().PaddingTop(2).AlignCenter().Text("(ünvan)").FontSize(10);

                        appCol.Item().PaddingTop(10);

                        // ── Field 2: name ─────────────────────────────────────
                        // italic value right-aligned (wraps if long)
                        appCol.Item().AlignRight().Text($"{fullName} {gender}").Italic();
                        // "ünvanda qeydiyyatda olan" on left + underline on right
                        appCol.Item().PaddingTop(4).Row(row =>
                        {
                            row.AutoItem().Text("ünvanda qeydiyyatda olan ");
                            row.RelativeItem().BorderBottom(0.5f).BorderColor(Colors.Black);
                        });
                        appCol.Item().PaddingTop(2).AlignRight().Text("(ad, soyad, ata adı)").FontSize(10);

                        appCol.Item().PaddingTop(10);

                        // ── Field 3: phone ────────────────────────────────────
                        // mobile / home (if present), then underline + "Tərəfindən", then label
                        appCol.Item().Text(phoneDisplay).Italic();
                        appCol.Item().PaddingTop(4).Row(row =>
                        {
                            row.RelativeItem().BorderBottom(0.5f).BorderColor(Colors.Black);
                            row.AutoItem().PaddingLeft(6).Text("tərəfindən");
                        });
                        appCol.Item().PaddingTop(2).Text("(tel: ev və mobil nömrə)").FontSize(10);
                    });

                    // ── Spacer ───────────────────────────────────────────────
                    column.Item().PaddingTop(40);

                    // ── ƏRİZƏ № title ─ center, BOLD ────────────────────────
                    column.Item().AlignCenter().Text(text =>
                    {
                        text.Span($"Ə R İ Z Ə  №{ja.ApplicationNumber:D4}").Bold();
                    });

                    column.Item().PaddingTop(20);

                    // ── Body paragraph ─ justified, double line-height ───────
                    column.Item().Text(text =>
                    {
                        text.DefaultTextStyle(s => s.LineHeight(2f));
                        text.Justify();
                        text.Span("   Yazıb sizdən xahiş edirəm ki, məni ");
                        text.Span(ja.StartDate.ToString("dd.MM.yyyy")).Underline();
                        text.Span($"-{yearSuffix} il tarixdən etibarən ");
                        text.Span((ja.Job?.Name ?? "").ToLowerInvariant()).Underline();
                        text.Span(" vəzifəsinə işə qəbul edəsiniz.");
                    });

                    column.Item().PaddingTop(50);

                    // ── Date line ────────────────────────────────────────────
                    column.Item().Text(
                        $"\"{ja.CreatedAt.Day:D2}\"  {GetMonthName(ja.CreatedAt.Month)}  {ja.CreatedAt.Year}-{yearSuffix} il");

                    column.Item().PaddingTop(20);

                    // ── Signature section ────────────────────────────────────
                    // Left: İmza + blank (for handwritten signature) + label
                    // Right: full name italic on underline + (ad, soyad, ata adı) label
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
        return (stream, ja.ApplicationNumber);
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
