namespace MTK.Modules.Payments.Application.Reports.GetMonthlyFinancialReport;

/// <summary>
/// "Aylıq hesabat"ın maliyyə hissəsi. Bütün məbləğlər AZN-dir.
/// Büdcə göstəriciləri ayrıca budget cədvəli olmadığı üçün mövcud
/// charge/payment/transaction məlumatlarından hesablanır — düsturlar
/// <see cref="GetMonthlyFinancialReportQueryHandler"/> və
/// <c>MonthlyFinancialReportReader</c>-də sənədləşdirilib.
/// </summary>
public sealed record MonthlyFinancialReportResponse(
    int Year,
    int Month,
    string CompanyName,
    string? CompanyAddress,
    string? CompanyDirector,
    decimal TotalBudget,
    decimal CurrentBudget,
    decimal ReserveBudget,
    decimal AdvanceBalance,
    decimal CarryOver,
    decimal MonthIncome,
    decimal MonthExpenses,
    decimal MonthNet,
    decimal MonthCharges,
    decimal MonthPaid,
    decimal MonthDebt,
    decimal TotalOutstandingDebt,
    int PaymentCount,
    int ExpenseCount,
    int AdvanceOwnerCount,
    ApartmentReportSummary Apartments,
    GarageReportSummary Garages,
    IReadOnlyCollection<ExpenseCategoryRow> ExpenseBreakdown,
    IReadOnlyCollection<AdvanceRow> Advances,
    IReadOnlyCollection<DebtorRow> Debtors,
    IReadOnlyCollection<ExpenseDetailRow> ExpenseDetails,
    IReadOnlyCollection<PayrollPaymentRow> PayrollPayments);

public sealed record ApartmentReportSummary(
    int RegisteredCount,
    int UnpaidCount,
    decimal TotalArea,
    decimal? RatePerSquareMeter,
    decimal MonthlyNorm,
    decimal TotalDebt);

public sealed record GarageReportSummary(
    int RegisteredCount,
    int UnpaidCount,
    decimal MonthlyNorm,
    decimal TotalDebt,
    IReadOnlyCollection<GarageRateRow> Rates);

public sealed record GarageRateRow(string Type, decimal Amount);

public sealed record ExpenseCategoryRow(string Category, int Count, decimal Amount);

/// <summary>
/// Ay sonuna istifadə olunmamış ödəniş qalığı olan sakin. Sistemdə avans
/// ayrıca "gələcək ay" kimi saxlanmır — ödəniş borclara FIFO ilə tətbiq olunur,
/// qalıq isə sakinin hesabında avans kimi qalır.
/// </summary>
public sealed record AdvanceRow(
    string OwnerName,
    string Properties,
    decimal Amount);

public sealed record DebtorRow(
    string PropertyType,
    string Number,
    string? Building,
    string OwnerName,
    decimal? AreaSquareMeters,
    decimal MonthlyAmount,
    decimal PaidAmount,
    string UnpaidPeriods,
    decimal TotalDebt);

public sealed record ExpenseDetailRow(
    DateTimeOffset Date,
    string Category,
    string? Description,
    decimal Amount);

public sealed record PayrollPaymentRow(
    string EmployeeName,
    int Year,
    int Month,
    decimal GrossSalary,
    string PaymentMethod,
    DateTimeOffset PaymentDate);
