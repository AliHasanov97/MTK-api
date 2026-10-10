using MTK.Modules.Hr.Application.Abstractions.Organization;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.JobApplications;
using MTK.Modules.Hr.Domain.NoticesOfChangeInWorkingConditions;

namespace MTK.Modules.Hr.Infrastructure.Export;

internal sealed class NoticeOfChangeInWorkingConditionsWordExportService
{
    private readonly IOrganizationInfo _organization;

    private readonly INoticeOfChangeInWorkingConditionsRepository _repository;

    public NoticeOfChangeInWorkingConditionsWordExportService(INoticeOfChangeInWorkingConditionsRepository repository,
        IOrganizationInfo organization)
    {
        _organization = organization;
        _repository = repository;
    }

    public async Task<(MemoryStream Stream, int Index)> ExportToWordAsync(
        Guid noticeId,
        CancellationToken cancellationToken = default)
    {
        var notice = await _repository.GetByIdDefaultAsync(noticeId, cancellationToken);
        if (notice == null)
        {
            throw new InvalidOperationException($"Notice with id {noticeId} not found");
        }

        var stream = new MemoryStream();

        // Data extraction
        var employee = notice.Employee!;
        var fullName = $"{employee.Name} {employee.Surname} {employee.FathersName}";
        var toGender = employee.Gender == Gender.Male ? "oğluna" : "qızına";
        var gender = employee.Gender == Gender.Male ? "oğlu" : "qızı";
        var companyName = _organization.Name;
        var companyDirector = _organization.Director ?? string.Empty;
        var jobName = employee.Job?.Name ?? string.Empty;
        var startDate = notice.StartDate;
        var noticeNumber = notice.Index;

        // Format dates
        var day = startDate.Day;
        var month = startDate.Month;
        var year = startDate.Year;
        var monthName = GetMonthName(month);

        var createdDay = notice.CreatedAt.Day;
        var createdMonth = notice.CreatedAt.Month;
        var createdYear = notice.CreatedAt.Year;
        var createdMonthName = GetMonthName(createdMonth);

        using (var document = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document))
        {
            var mainPart = document.AddMainDocumentPart();
            mainPart.Document = new Document();
            var body = new Body();

            // ===== BAŞLIQ =====
            body.AppendChild(CreateRightAlignedParagraph($"{companyName}-də", bold: true));

            // 1ci - Peşə adı (sağ tərəfdə)
            body.AppendChild(CreateRightAlignedParagraph($"{jobName.ToLowerInvariant()}"));

            // peşə üzrə çalışan (sağ tərəfdə)
            body.AppendChild(CreateRightAlignedParagraph("peşə üzrə çalışan"));

            // 2ci - İşçinin kimliyi (sağ tərəfdə)
            body.AppendChild(CreateRightAlignedParagraph($"{fullName} {toGender}"));
            body.AppendChild(CreateEmptyParagraph());

            // Əmək şəraitinin dəyişdirilməsi barədə
            body.AppendChild(CreateCenteredParagraph("Əmək şəraitinin dəyişdirilməsi barədə", italic: true));
            body.AppendChild(CreateEmptyParagraph());

            // XƏBƏRDARLIQ №
            body.AppendChild(CreateCenteredParagraph($"XƏBƏRDARLIQ № {noticeNumber}", bold: true));
            body.AppendChild(CreateEmptyParagraph());

            // ===== ƏSAS MƏTN =====
            var mainText = $"İstehsalatın təşkilində dəyişikliklər edilməsi zəruriyyəti ilə əlaqədar olaraq, " +
                          $"Sizin çalışdığınız {jobName.ToLowerInvariant()} peşəsi üzrə əmək şəraitinin şərtləri " +
                          $"dəyişdirilərək {day:D2}.{month:D2}.{year}-ci il tarixdən etibarən aşağıdakı qaydada müəyyən edilir:";

            body.AppendChild(CreateJustifiedParagraph(mainText));
            body.AppendChild(CreateEmptyParagraph());

            // ===== EDITABLE HİSSƏ - YALNIZ MADDƏLƏR =====
            body.AppendChild(new Paragraph(
                new PermStart { Id = 0, EditorGroup = RangePermissionEditingGroupValues.Everyone }
            ));

            // 5-6 boş sətir editable üçün
            for (int i = 0; i < 6; i++)
            {
                body.AppendChild(CreateEmptyParagraph());
            }

            body.AppendChild(new Paragraph(
                new PermEnd { Id = 0 }
            ));
            // ===== SON EDITABLE HİSSƏ =====

         

           

            // ===== İMZA HİSSƏSİ =====
            var signatureTable = CreateSignatureTable(companyName, companyDirector);
            body.AppendChild(signatureTable);
            body.AppendChild(CreateEmptyParagraph());

            // Xəbərdarlıq məktubunu aldım:
            body.AppendChild(CreateLeftAlignedParagraph("Xəbərdarlıq məktubunu aldım:"));
            body.AppendChild(CreateEmptyParagraph());
          

            // İmza
            body.AppendChild(CreateLeftAlignedParagraph($"İmza: _____________  {fullName} {gender}"));
            body.AppendChild(CreateEmptyParagraph());
            body.AppendChild(CreateLeftAlignedParagraph("                                            (ad soyad)"));
            body.AppendChild(CreateEmptyParagraph());

            // Tarix
            body.AppendChild(CreateLeftAlignedParagraph($"Tarix: {createdDay} {createdMonthName} {createdYear}-ci il"));

            // Section Properties (A4, margins)
            body.AppendChild(CreateSectionProperties());

            mainPart.Document.AppendChild(body);

            // ===== DOCUMENT PROTECTION (ReadOnly, amma yalnız maddələr editable) =====
            var settingsPart = mainPart.AddNewPart<DocumentSettingsPart>();
            settingsPart.Settings = new Settings(
                new DocumentProtection
                {
                    Edit = DocumentProtectionValues.ReadOnly,
                    Enforcement = OnOffValue.FromBoolean(true)
                }
            );
        }

        stream.Position = 0;
        return (stream, notice.Index);
    }

    private static Paragraph CreateCenteredParagraph(string text, bool bold = false, bool italic = false)
    {
        var paragraph = new Paragraph(
            new ParagraphProperties(
                new Justification { Val = JustificationValues.Center },
                new SpacingBetweenLines { After = "0" }
            )
        );

        var rPr = new RunProperties(
            new FontSize { Val = "24" },
            new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" }
        );

        if (bold) rPr.AppendChild(new Bold());
        if (italic) rPr.AppendChild(new Italic());

        var run = new Run(rPr, new Text(text));
        paragraph.AppendChild(run);

        return paragraph;
    }

    private static Paragraph CreateLeftAlignedParagraph(string text, bool bold = false)
    {
        var paragraph = new Paragraph(
            new ParagraphProperties(
                new Justification { Val = JustificationValues.Left },
                new SpacingBetweenLines { After = "0" }
            )
        );

        var rPr = new RunProperties(
            new FontSize { Val = "24" },
            new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" }
        );

        if (bold) rPr.AppendChild(new Bold());

        var run = new Run(rPr, new Text(text));
        paragraph.AppendChild(run);

        return paragraph;
    }

    private static Paragraph CreateRightAlignedParagraph(string text, bool bold = false)
    {
        var paragraph = new Paragraph(
            new ParagraphProperties(
                new Justification { Val = JustificationValues.Right },
                new SpacingBetweenLines { After = "0" }
            )
        );

        var rPr = new RunProperties(
            new FontSize { Val = "24" },
            new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" }
        );

        if (bold) rPr.AppendChild(new Bold());

        var run = new Run(rPr, new Text(text));
        paragraph.AppendChild(run);

        return paragraph;
    }

    private static Paragraph CreateJustifiedParagraph(string text)
    {
        var paragraph = new Paragraph(
            new ParagraphProperties(
                new Justification { Val = JustificationValues.Both },
                new SpacingBetweenLines { After = "200", Line = "276", LineRule = LineSpacingRuleValues.Auto }
            )
        );

        var run = new Run(
            new RunProperties(
                new FontSize { Val = "24" },
                new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" }
            ),
            new Text(text) { Space = SpaceProcessingModeValues.Preserve }
        );

        paragraph.AppendChild(run);
        return paragraph;
    }

    private static Paragraph CreateEmptyParagraph()
    {
        return new Paragraph(
            new Run(
                new RunProperties(
                    new FontSize { Val = "24" },
                    new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" }
                ),
                new Text("")
            )
        );
    }

    private static Table CreateSignatureTable(string companyName, string companyDirector)
    {
        var table = new Table();

        table.AppendChild(new TableProperties(
            new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct },
            new TableBorders(
                new TopBorder { Val = BorderValues.None },
                new BottomBorder { Val = BorderValues.None },
                new LeftBorder { Val = BorderValues.None },
                new RightBorder { Val = BorderValues.None },
                new InsideHorizontalBorder { Val = BorderValues.None },
                new InsideVerticalBorder { Val = BorderValues.None }
            )
        ));

        var row = new TableRow();

        // Sol xana - Company MMC
        var leftCell = new TableCell(
            new TableCellProperties(new TableCellWidth { Width = "2500", Type = TableWidthUnitValues.Pct }),
            new Paragraph(
                new ParagraphProperties(new Justification { Val = JustificationValues.Left }),
                new Run(
                    new RunProperties(
                        new FontSize { Val = "24" },
                        new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" }
                    ),
                    new Text(companyName)
                )
            )
        );

        // Sağ xana - Şirkət direktoru
        var rightCell = new TableCell(
            new TableCellProperties(new TableCellWidth { Width = "2500", Type = TableWidthUnitValues.Pct }),
            new Paragraph(
                new ParagraphProperties(new Justification { Val = JustificationValues.Right }),
                new Run(
                    new RunProperties(
                        new FontSize { Val = "24" },
                        new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" }
                    ),
                    new Text(companyDirector)
                )
            )
        );

        row.AppendChild(leftCell);
        row.AppendChild(rightCell);
        table.AppendChild(row);

        return table;
    }

    private static string GetMonthName(int month)
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

    private static SectionProperties CreateSectionProperties()
    {
        return new SectionProperties(
            new PageSize { Width = 11906, Height = 16838, Orient = PageOrientationValues.Portrait },
            new PageMargin { Top = 1134, Right = 850, Bottom = 1134, Left = 1701, Header = 720, Footer = 720, Gutter = 0 }
        );
    }
}
