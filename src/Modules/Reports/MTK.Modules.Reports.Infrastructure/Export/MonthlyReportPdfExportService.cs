using System.Globalization;
using MTK.Modules.Reports.Application.Abstractions;
using MTK.Modules.Reports.Application.MonthlyReports;
using MTK.Modules.Warehouse.Application.Reports.GetWarehouseMonthlyReport;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MTK.Modules.Reports.Infrastructure.Export;

/// <summary>
/// "Şəffaf İdarəetmə" üslubunda aylıq hesabat: başlıq, KPI kartları, xülasə,
/// tariflər, xərclər, avanslar, borclular, əməkhaqqı və anbar bölmələri.
/// Rəqəmlər yalnız gələn read modeldən götürülür — burada heç bir hesablama
/// təkrar edilmir (büdcə düsturları Payments-in read modelindədir).
/// </summary>
internal sealed class MonthlyReportPdfExportService : IMonthlyReportPdfExportService
{
    internal const string Navy = "#0F2A43";
    internal const string Accent = "#1D4E89";
    internal const string Rule = "#D7E0EA";
    internal const string Muted = "#5B6B7C";
    internal const string TextColor = "#1E293B";
    internal const string SoftBackground = "#F7F9FC";

    private static readonly CultureInfo Az = CreateAzCulture();

    private static readonly string[] AzMonths =
    [
        "Yanvar", "Fevral", "Mart", "Aprel", "May", "İyun",
        "İyul", "Avqust", "Sentyabr", "Oktyabr", "Noyabr", "Dekabr",
    ];

    public MemoryStream ExportToPdf(MonthlyReportResponse report)
    {
        var stream = new MemoryStream();

        Document.Create(container => container.Page(page => ComposePage(page, report)))
            .GeneratePdf(stream);

        stream.Position = 0;
        return stream;
    }

    private static void ComposePage(PageDescriptor page, MonthlyReportResponse report)
    {
        var finance = report.Finance;
        var warehouse = report.Warehouse;
        var monthLabel = $"{AzMonths[finance.Month - 1]} {finance.Year}";

        page.Size(PageSizes.A4);
        page.Margin(34);
        page.PageColor(Colors.White);
        page.DefaultTextStyle(x => x.FontFamily("Times New Roman").FontSize(8.5f).FontColor(TextColor));

        page.Footer().PaddingTop(8).Row(row =>
        {
            row.RelativeItem().Text(
                    $"Bu sənəd elektron qaydada yaradılıb · {report.GeneratedAt:dd.MM.yyyy HH:mm}")
                .FontSize(7f).FontColor(Muted).Italic();

            row.ConstantItem(90).AlignRight()
                .DefaultTextStyle(style => style.FontSize(7f).FontColor(Muted))
                .Text(text =>
                {
                    text.CurrentPageNumber();
                    text.Span(" / ");
                    text.TotalPages();
                });
        });

        page.Content().Column(column =>
        {
            ComposeHeader(column, finance.CompanyName, finance.CompanyAddress, finance.CompanyDirector, monthLabel);

            // ---- Büdcə kartları -------------------------------------------------
            Section(column, "Büdcə xülasəsi");
            KpiRow(column,
                ("Ümumi büdcə", Money(finance.TotalBudget), "cari + ehtiyat + avans"),
                ("Cari büdcə", Money(finance.CurrentBudget), "cari ay üzrə yığım"),
                ("Ehtiyat büdcə", Money(finance.ReserveBudget), "ay sonu qalıq"));
            KpiRow(column,
                ("Aylıq ödəniş norması", Money(finance.MonthCharges), "cari ay üzrə hesablanmış"),
                ("Avans qalığı", Money(finance.AdvanceBalance), "istifadə olunmamış ödənişlər"),
                ("Əvvəlki aylardan qalıq", Money(finance.CarryOver), "ay başına qədər gəlir − xərc"));

            column.Item().PaddingTop(4).Text(
                    "Ümumi büdcə = cari büdcə + ehtiyat büdcə + avans qalığı. " +
                    "Büdcə ayrıca cədvəl kimi saxlanmır; göstəricilər charge, payment və ledger məlumatlarından hesablanır.")
                .FontSize(7f).FontColor(Muted).Italic();

            // ---- Xülasə ---------------------------------------------------------
            Section(column, "Hesabat xülasəsi");
            SummaryTable(column,
            [
                ("Əvvəlki aylardan qalıq", finance.CarryOver, false),
                ("Cari ay üzrə hesablanmış borc", finance.MonthCharges, false),
                ("Cari ay üzrə ödənilmiş", finance.MonthPaid, false),
                ("Cari ay üzrə qalıq borc", finance.MonthDebt, true),
                ("Bu ay daxil olan ödənişlər", finance.MonthIncome, false),
                ("Bu ay xərclər", finance.MonthExpenses, false),
                ("Aylıq xalis nəticə", finance.MonthNet, true),
                ("Ay sonuna MTK üzrə ümumi borc", finance.TotalOutstandingDebt, true),
            ]);

            // ---- Sahə və tariflər ----------------------------------------------
            Section(column, "Sahə və tariflər");
            RatesTable(column, finance);

            // ---- Göstəricilər ---------------------------------------------------
            Section(column, "Göstəricilər");
            MetricsTable(column, finance);

            // ---- Xərc bölgüsü ---------------------------------------------------
            Section(column, "Xərclərin bölgüsü");
            ExpenseBreakdownTable(column, finance.ExpenseBreakdown);

            // ---- Avanslar -------------------------------------------------------
            Section(column, "Avanslar");
            AdvancesTable(column, finance.Advances, finance.AdvanceBalance);

            // ---- Borclular ------------------------------------------------------
            Section(column, "Ödəniş etməyən mənzil və qarajlar");
            DebtorsTable(column, finance.Debtors);

            // ---- Xərc detalları -------------------------------------------------
            Section(column, "Xərclərin detalları");
            ExpenseDetailsTable(column, finance.ExpenseDetails);

            // ---- Əməkhaqqı ------------------------------------------------------
            Section(column, "Əməkhaqqı ödənişləri");
            PayrollTable(column, finance.PayrollPayments);

            // ---- Anbar ----------------------------------------------------------
            Section(column, "Anbar hesabatı");
            WarehouseSection(column, warehouse);
        });
    }

    private static void ComposeHeader(
        ColumnDescriptor column,
        string companyName,
        string? address,
        string? director,
        string monthLabel)
    {
        column.Item().Row(row =>
        {
            row.RelativeItem().Column(head =>
            {
                head.Item().Text(companyName.ToUpper(Az)).FontSize(8.5f).FontColor(Accent).LetterSpacing(0.08f);
                head.Item().Text("AYLIQ HESABAT").FontSize(22).Bold().FontColor(Navy);
                head.Item().Text("Şəffaflıq · Dəqiq uçot · Etibarlı nəzarət")
                    .FontSize(7.5f).FontColor(Muted).LetterSpacing(0.03f);
            });

            row.ConstantItem(155).AlignRight().Column(meta =>
            {
                meta.Item().AlignRight().Text(monthLabel).FontSize(13).Bold().FontColor(Navy);
                if (!string.IsNullOrWhiteSpace(director))
                {
                    meta.Item().AlignRight().Text($"Direktor: {director}").FontSize(7f).FontColor(Muted);
                }

                if (!string.IsNullOrWhiteSpace(address))
                {
                    meta.Item().AlignRight().Text(address).FontSize(7f).FontColor(Muted);
                }
            });
        });

        column.Item().PaddingTop(6).LineHorizontal(1.4f).LineColor(Navy);
    }

    private static void Section(ColumnDescriptor column, string title)
    {
        column.Item().PaddingTop(12).Text(title).FontSize(11.5f).Bold().FontColor(Navy);
        column.Item().PaddingTop(2).LineHorizontal(0.9f).LineColor(Navy);
        column.Item().Height(4);
    }

    private static void KpiRow(ColumnDescriptor column, params (string Label, string Value, string Note)[] cards)
    {
        column.Item().PaddingTop(4).Row(row =>
        {
            row.Spacing(6);

            foreach (var (label, value, note) in cards)
            {
                KpiCard(row.RelativeItem(), label, value, note);
            }
        });
    }

    private static void KpiCard(IContainer container, string label, string value, string note)
    {
        container
            .MinHeight(60)
            .Border(0.6f).BorderColor(Rule)
            .Background(SoftBackground)
            .Padding(8)
            .Column(column =>
            {
                column.Item().Text(label).FontSize(7.2f).FontColor(Muted);
                column.Item().PaddingTop(2).Text(value).FontSize(12.5f).Bold().FontColor(Navy);
                column.Item().PaddingTop(2).Text(note).FontSize(6.6f).FontColor(Muted);
            });
    }

    private static void SummaryTable(
        ColumnDescriptor column,
        IReadOnlyCollection<(string Label, decimal Value, bool Bold)> rows)
    {
        column.Item().PaddingTop(3).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(3);
                columns.RelativeColumn(1.2f);
            });

            foreach (var (label, value, bold) in rows)
            {
                table.Cell().PdfSummaryCell(bold).Text(label);
                table.Cell().PdfSummaryCell(bold)
                    .AlignRight().Text(Money(value));
            }
        });
    }

    private static void RatesTable(
        ColumnDescriptor column,
        MTK.Modules.Payments.Application.Reports.GetMonthlyFinancialReport.MonthlyFinancialReportResponse finance)
    {
        column.Item().PaddingTop(3).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(2.4f);
                columns.RelativeColumn(1.2f);
                columns.RelativeColumn(1.2f);
            });

            table.Header(header =>
            {
                header.Cell().PdfHeaderCell().Text("Göstərici");
                header.Cell().PdfHeaderCell().AlignRight().Text("Mənzillər");
                header.Cell().PdfHeaderCell().AlignRight().Text("Qarajlar");
            });

            var apartments = finance.Apartments;
            var garages = finance.Garages;

            Row(table, "Qeydiyyatda olan", apartments.RegisteredCount.ToString(), garages.RegisteredCount.ToString());
            Row(table, "Ümumi sahə (m²)", Qty(apartments.TotalArea), "—");
            Row(table, "Tarif",
                apartments.RatePerSquareMeter is { } rate ? $"{Money(rate)} / m²" : "—",
                garages.Rates.Count == 0
                    ? "—"
                    : string.Join(" · ", garages.Rates.Select(r => $"{r.Type}: {Money(r.Amount)}")));
            Row(table, "Aylıq hesablanmış", Money(apartments.MonthlyNorm), Money(garages.MonthlyNorm));
            Row(table, "Ödəniş etməyən", apartments.UnpaidCount.ToString(), garages.UnpaidCount.ToString());
            Row(table, "Ümumi borc", Money(apartments.TotalDebt), Money(garages.TotalDebt), bold: true);

            static void Row(TableDescriptor table, string label, string apartments, string garages, bool bold = false)
            {
                table.Cell().PdfDataCell(bold).Text(label);
                table.Cell().PdfDataCell(bold).AlignRight().Text(apartments);
                table.Cell().PdfDataCell(bold).AlignRight().Text(garages);
            }
        });
    }

    private static void MetricsTable(
        ColumnDescriptor column,
        MTK.Modules.Payments.Application.Reports.GetMonthlyFinancialReport.MonthlyFinancialReportResponse finance)
    {
        var metrics = new (string Label, string Value)[]
        {
            ("Qeyd alınmış ödəniş sayı", finance.PaymentCount.ToString()),
            ("Qeyd alınmış xərc sayı", finance.ExpenseCount.ToString()),
            ("Avansı olan sakin sayı", finance.AdvanceOwnerCount.ToString()),
            ("Borclu mənzil sayı", finance.Apartments.UnpaidCount.ToString()),
            ("Borclu qaraj sayı", finance.Garages.UnpaidCount.ToString()),
            ("Cari ay üzrə qalıq borc", Money(finance.MonthDebt)),
            ("Ay sonuna MTK üzrə ümumi borc", Money(finance.TotalOutstandingDebt)),
            ("Əvvəlki aylardan qalıq", Money(finance.CarryOver)),
        };

        column.Item().PaddingTop(3).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(2.4f);
                columns.RelativeColumn(1.1f);
                columns.RelativeColumn(2.4f);
                columns.RelativeColumn(1.1f);
            });

            for (var i = 0; i < metrics.Length; i += 2)
            {
                AddMetric(table, metrics[i]);
                if (i + 1 < metrics.Length)
                {
                    AddMetric(table, metrics[i + 1]);
                }
                else
                {
                    table.Cell().PdfDataCell().Text(string.Empty);
                    table.Cell().PdfDataCell().Text(string.Empty);
                }
            }

            static void AddMetric(TableDescriptor table, (string Label, string Value) metric)
            {
                table.Cell().PdfDataCell().Text(metric.Label);
                table.Cell().PdfDataCell().AlignRight().Text(metric.Value).Bold();
            }
        });
    }

    private static void ExpenseBreakdownTable(
        ColumnDescriptor column,
        IReadOnlyCollection<MTK.Modules.Payments.Application.Reports.GetMonthlyFinancialReport.ExpenseCategoryRow> rows)
    {
        if (rows.Count == 0)
        {
            EmptyNote(column, "Seçilmiş ay üzrə xərc qeydə alınmayıb.");
            return;
        }

        column.Item().PaddingTop(3).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(3);
                columns.RelativeColumn(1);
                columns.RelativeColumn(1.4f);
            });

            table.Header(header =>
            {
                header.Cell().PdfHeaderCell().Text("Kateqoriya");
                header.Cell().PdfHeaderCell().AlignRight().Text("Say");
                header.Cell().PdfHeaderCell().AlignRight().Text("Məbləğ");
            });

            foreach (var row in rows)
            {
                table.Cell().PdfDataCell().Text(row.Category);
                table.Cell().PdfDataCell().AlignRight().Text(row.Count.ToString());
                table.Cell().PdfDataCell().AlignRight().Text(Money(row.Amount));
            }

            table.Cell().PdfTotalCell().Text("Cəmi");
            table.Cell().PdfTotalCell().AlignRight().Text(rows.Sum(r => r.Count).ToString());
            table.Cell().PdfTotalCell().AlignRight().Text(Money(rows.Sum(r => r.Amount)));
        });
    }

    private static void AdvancesTable(
        ColumnDescriptor column,
        IReadOnlyCollection<MTK.Modules.Payments.Application.Reports.GetMonthlyFinancialReport.AdvanceRow> rows,
        decimal total)
    {
        if (rows.Count == 0)
        {
            EmptyNote(column, "Ay sonuna istifadə olunmamış avans qalığı yoxdur.");
            return;
        }

        column.Item().PaddingTop(3).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(2);
                columns.RelativeColumn(2.4f);
                columns.RelativeColumn(1.3f);
            });

            table.Header(header =>
            {
                header.Cell().PdfHeaderCell().Text("Sakin");
                header.Cell().PdfHeaderCell().Text("Əmlak");
                header.Cell().PdfHeaderCell().AlignRight().Text("Avans qalığı");
            });

            foreach (var row in rows)
            {
                table.Cell().PdfDataCell().Text(row.OwnerName);
                table.Cell().PdfDataCell().Text(row.Properties);
                table.Cell().PdfDataCell().AlignRight().Text(Money(row.Amount));
            }

            table.Cell().PdfTotalCell().Text("Cəmi");
            table.Cell().PdfTotalCell().Text(string.Empty);
            table.Cell().PdfTotalCell().AlignRight().Text(Money(total));
        });

        column.Item().PaddingTop(2).Text(
                "Avans ayrıca gələcək aylara bölünmür: ödəniş borclara FIFO qaydası ilə tətbiq olunur, " +
                "qalıq isə sakinin hesabında saxlanılır.")
            .FontSize(7f).FontColor(Muted).Italic();
    }

    private static void DebtorsTable(
        ColumnDescriptor column,
        IReadOnlyCollection<MTK.Modules.Payments.Application.Reports.GetMonthlyFinancialReport.DebtorRow> rows)
    {
        if (rows.Count == 0)
        {
            EmptyNote(column, "Seçilmiş ay üzrə 2,00 ₼-dan yuxarı borclu mənzil/qaraj yoxdur.");
            return;
        }

        column.Item().PaddingTop(3).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(1.8f);
                columns.RelativeColumn(2f);
                columns.RelativeColumn(1f);
                columns.RelativeColumn(1f);
                columns.RelativeColumn(2f);
                columns.RelativeColumn(1.2f);
            });

            table.Header(header =>
            {
                header.Cell().PdfHeaderCell().Text("Əmlak");
                header.Cell().PdfHeaderCell().Text("Sahib");
                header.Cell().PdfHeaderCell().AlignRight().Text("Aylıq");
                header.Cell().PdfHeaderCell().AlignRight().Text("Ödənilib");
                header.Cell().PdfHeaderCell().Text("Borc olduğu aylar");
                header.Cell().PdfHeaderCell().AlignRight().Text("Ümumi borc");
            });

            foreach (var row in rows)
            {
                table.Cell().PdfDataCell().Text(text =>
                {
                    text.Span($"{row.PropertyType} {row.Number}").Bold();
                    if (!string.IsNullOrWhiteSpace(row.Building))
                    {
                        text.Span($"\n{row.Building}");
                    }

                    if (row.AreaSquareMeters is { } area)
                    {
                        text.Span($"\n{Qty(area)} m²");
                    }
                });
                table.Cell().PdfDataCell().Text(row.OwnerName);
                table.Cell().PdfDataCell().AlignRight().Text(Money(row.MonthlyAmount));
                table.Cell().PdfDataCell().AlignRight().Text(Money(row.PaidAmount));
                table.Cell().PdfDataCell().Text(row.UnpaidPeriods);
                table.Cell().PdfDataCell().AlignRight().Text(Money(row.TotalDebt)).Bold();
            }
        });

        column.Item().PaddingTop(2).Text(
                "Bu siyahıya yalnız 2,00 ₼-dan yuxarı cari ay borcu olan əmlaklar daxil edilir.")
            .FontSize(7f).FontColor(Muted).Italic();
    }

    private static void ExpenseDetailsTable(
        ColumnDescriptor column,
        IReadOnlyCollection<MTK.Modules.Payments.Application.Reports.GetMonthlyFinancialReport.ExpenseDetailRow> rows)
    {
        if (rows.Count == 0)
        {
            EmptyNote(column, "Seçilmiş ay üzrə xərc əməliyyatı yoxdur.");
            return;
        }

        column.Item().PaddingTop(3).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(1.1f);
                columns.RelativeColumn(1.6f);
                columns.RelativeColumn(3.5f);
                columns.RelativeColumn(1.2f);
            });

            table.Header(header =>
            {
                header.Cell().PdfHeaderCell().Text("Tarix");
                header.Cell().PdfHeaderCell().Text("Kateqoriya");
                header.Cell().PdfHeaderCell().Text("Təsvir");
                header.Cell().PdfHeaderCell().AlignRight().Text("Məbləğ");
            });

            foreach (var row in rows)
            {
                table.Cell().PdfDataCell().Text(row.Date.ToString("dd.MM.yyyy"));
                table.Cell().PdfDataCell().Text(row.Category);
                table.Cell().PdfDataCell().Text(string.IsNullOrWhiteSpace(row.Description) ? "—" : row.Description);
                table.Cell().PdfDataCell().AlignRight().Text(Money(row.Amount));
            }

            table.Cell().PdfTotalCell().Text("Cəmi");
            table.Cell().PdfTotalCell().Text(string.Empty);
            table.Cell().PdfTotalCell().Text(string.Empty);
            table.Cell().PdfTotalCell().AlignRight().Text(Money(rows.Sum(r => r.Amount)));
        });
    }

    private static void PayrollTable(
        ColumnDescriptor column,
        IReadOnlyCollection<MTK.Modules.Payments.Application.Reports.GetMonthlyFinancialReport.PayrollPaymentRow> rows)
    {
        if (rows.Count == 0)
        {
            EmptyNote(column, "Seçilmiş ay üzrə əməkhaqqı ödənişi yoxdur.");
            return;
        }

        column.Item().PaddingTop(3).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(2.2f);
                columns.RelativeColumn(1.2f);
                columns.RelativeColumn(1.2f);
                columns.RelativeColumn(1.2f);
                columns.RelativeColumn(1.2f);
            });

            table.Header(header =>
            {
                header.Cell().PdfHeaderCell().Text("İşçi");
                header.Cell().PdfHeaderCell().Text("Dövr");
                header.Cell().PdfHeaderCell().AlignRight().Text("Gross");
                header.Cell().PdfHeaderCell().Text("Metod");
                header.Cell().PdfHeaderCell().Text("Tarix");
            });

            foreach (var row in rows)
            {
                table.Cell().PdfDataCell().Text(row.EmployeeName);
                table.Cell().PdfDataCell().Text($"{AzMonths[row.Month - 1]} {row.Year}");
                table.Cell().PdfDataCell().AlignRight().Text(Money(row.GrossSalary));
                table.Cell().PdfDataCell().Text(row.PaymentMethod);
                table.Cell().PdfDataCell().Text(row.PaymentDate.ToString("dd.MM.yyyy"));
            }

            table.Cell().PdfTotalCell().Text("Cəmi");
            table.Cell().PdfTotalCell().Text(string.Empty);
            table.Cell().PdfTotalCell().AlignRight().Text(Money(rows.Sum(r => r.GrossSalary)));
            table.Cell().PdfTotalCell().Text(string.Empty);
            table.Cell().PdfTotalCell().Text(string.Empty);
        });
    }

    private static void WarehouseSection(ColumnDescriptor column, WarehouseMonthlyReportResponse warehouse)
    {
        KpiRow(column,
            ("Faktiki qalıq", Qty(warehouse.CurrentStockQuantity), "cari stok miqdarı"),
            ("Mal/material sayı", warehouse.StockItemCount.ToString(), "stok sətirləri"),
            ("Uçot dəyəri", Money(warehouse.StockValue), "son qəbul qiymətləri ilə"));
        KpiRow(column,
            ("Mədaxil", Qty(warehouse.ReceiptQuantity), $"{warehouse.ReceiptCount} əməliyyat"),
            ("Məxaric", Qty(warehouse.IssueQuantity), $"{warehouse.IssueCount} əməliyyat"));

        column.Item().PaddingTop(6).Text("Faktiki mövcud olan mal və materiallar")
            .FontSize(9).Bold().FontColor(Accent);

        if (warehouse.Items.Count == 0)
        {
            EmptyNote(column, "Anbar boşdur.");
        }
        else
        {
            column.Item().PaddingTop(2).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(1.1f);
                    columns.RelativeColumn(2.4f);
                    columns.RelativeColumn(0.8f);
                    columns.RelativeColumn(1f);
                    columns.RelativeColumn(1f);
                    columns.RelativeColumn(1f);
                });

                table.Header(header =>
                {
                    header.Cell().PdfHeaderCell().Text("Kod");
                    header.Cell().PdfHeaderCell().Text("Mal / material");
                    header.Cell().PdfHeaderCell().Text("Vahid");
                    header.Cell().PdfHeaderCell().AlignRight().Text("Qalıq");
                    header.Cell().PdfHeaderCell().AlignRight().Text("Vahid qiymət");
                    header.Cell().PdfHeaderCell().AlignRight().Text("Dəyər");
                });

                foreach (var row in warehouse.Items)
                {
                    table.Cell().PdfDataCell().Text(row.Code);
                    table.Cell().PdfDataCell().Text(row.Name);
                    table.Cell().PdfDataCell().Text(row.Unit);
                    table.Cell().PdfDataCell().AlignRight().Text(Qty(row.Quantity));
                    table.Cell().PdfDataCell().AlignRight()
                        .Text(row.UnitPrice is null ? "—" : Money(row.UnitPrice.Value));
                    table.Cell().PdfDataCell().AlignRight()
                        .Text(row.Value is null ? "—" : Money(row.Value.Value));
                }

                table.Cell().PdfTotalCell().Text("Cəmi");
                table.Cell().PdfTotalCell().Text(string.Empty);
                table.Cell().PdfTotalCell().Text(string.Empty);
                table.Cell().PdfTotalCell().AlignRight().Text(Qty(warehouse.CurrentStockQuantity));
                table.Cell().PdfTotalCell().Text(string.Empty);
                table.Cell().PdfTotalCell().AlignRight().Text(Money(warehouse.StockValue));
            });
        }

        column.Item().PaddingTop(7).Text("Seçilmiş ayın mədaxil və məxaric hərəkətləri")
            .FontSize(9).Bold().FontColor(Accent);

        if (warehouse.Movements.Count == 0)
        {
            EmptyNote(column, "Seçilmiş ay üzrə anbar hərəkəti yoxdur.");
            return;
        }

        column.Item().PaddingTop(2).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(1.1f);
                columns.RelativeColumn(2.4f);
                columns.RelativeColumn(1f);
                columns.RelativeColumn(0.9f);
                columns.RelativeColumn(2.2f);
            });

            table.Header(header =>
            {
                header.Cell().PdfHeaderCell().Text("Tarix");
                header.Cell().PdfHeaderCell().Text("Mal / material");
                header.Cell().PdfHeaderCell().Text("Əməliyyat");
                header.Cell().PdfHeaderCell().AlignRight().Text("Miqdar");
                header.Cell().PdfHeaderCell().Text("Qeyd");
            });

            foreach (var row in warehouse.Movements)
            {
                table.Cell().PdfDataCell().Text(row.Date.ToString("dd.MM.yyyy"));
                table.Cell().PdfDataCell().Text(row.ItemName);
                table.Cell().PdfDataCell().Text(row.Type);
                table.Cell().PdfDataCell().AlignRight().Text(Qty(row.Quantity));
                table.Cell().PdfDataCell().Text(string.IsNullOrWhiteSpace(row.Note) ? "—" : row.Note);
            }
        });
    }

    private static void EmptyNote(ColumnDescriptor column, string message)
    {
        column.Item().PaddingTop(3).Text(message).FontSize(8.5f).Italic().FontColor(Muted);
    }

    private static string Money(decimal amount) => $"{amount.ToString("N2", Az)} ₼";

    private static string Qty(decimal amount) => amount.ToString("N2", Az);

    private static CultureInfo CreateAzCulture()
    {
        try
        {
            return CultureInfo.GetCultureInfo("az-AZ");
        }
        catch (CultureNotFoundException)
        {
            return CultureInfo.InvariantCulture;
        }
    }
}

/// <summary>
/// QuestPDF-də <c>Element(...)</c> yalnız dinamik komponentlər üçündür; statik
/// cədvəl hüceyrələrini təkrar istifadə etmək üçün bu kiçik extension-lar.
/// </summary>
internal static class MonthlyReportPdfStyles
{
    public static IContainer PdfHeaderCell(this IContainer container) =>
        container
            .Background(MonthlyReportPdfExportService.Navy)
            .PaddingVertical(4).PaddingHorizontal(4)
            .DefaultTextStyle(style => style.FontSize(7.2f).Bold().FontColor(Colors.White));

    public static IContainer PdfDataCell(this IContainer container, bool bold = false) =>
        container
            .BorderBottom(0.4f).BorderColor(MonthlyReportPdfExportService.Rule)
            .PaddingVertical(3.5f).PaddingHorizontal(4)
            .DefaultTextStyle(style => bold
                ? style.FontSize(7.8f).Bold().FontColor(MonthlyReportPdfExportService.Navy)
                : style.FontSize(7.8f).FontColor(MonthlyReportPdfExportService.TextColor));

    public static IContainer PdfTotalCell(this IContainer container) =>
        container
            .Background("#EDF2F7")
            .BorderTop(0.8f).BorderColor(MonthlyReportPdfExportService.Navy)
            .PaddingVertical(4).PaddingHorizontal(4)
            .DefaultTextStyle(style => style.FontSize(7.8f).Bold().FontColor(MonthlyReportPdfExportService.Navy));

    public static IContainer PdfSummaryCell(this IContainer container, bool bold) =>
        container
            .BorderBottom(0.4f).BorderColor(MonthlyReportPdfExportService.Rule)
            .PaddingVertical(4).PaddingHorizontal(4)
            .DefaultTextStyle(style => bold
                ? style.FontSize(8.6f).Bold().FontColor(MonthlyReportPdfExportService.Navy)
                : style.FontSize(8.6f).FontColor(MonthlyReportPdfExportService.TextColor));
}
