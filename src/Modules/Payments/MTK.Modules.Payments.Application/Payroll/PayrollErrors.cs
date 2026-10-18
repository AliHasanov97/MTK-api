using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Application.Payroll;

public static class PayrollErrors
{
    public static Error RunNotFound(Guid id) => new(
        "PayrollRun.NotFound",
        $"PayrollRun tapılmadı: {id}");

    public static readonly Error NotApproved = new(
        "PayrollRun.NotApproved",
        "Ödəniş yalnız təsdiqlənmiş (Approved) hesablama üçün edilə bilər — əvvəlcə təsdiqləyin");

    public static readonly Error NotCalculated = new(
        "PayrollRun.NotCalculated",
        "Yalnız hesablanmış (Calculated) run təsdiqlənə bilər");

    public static Error EmployeeNotInRun(Guid employeeId) => new(
        "PayrollRun.EmployeeNotInRun",
        $"Bu dövrdə {employeeId} işçisi üçün qəbz tapılmadı");

    public static readonly Error AlreadyPaid = new(
        "PayrollRun.AlreadyPaid",
        "Bu qəbz artıq ödənilib");

    public static readonly Error NothingToPay = new(
        "PayrollRun.NothingToPay",
        "Bu run üzrə bütün qəbzlər artıq ödənilib");

    /// <summary>
    /// Əmək haqqı ay bitdikdən sonra hesablanır — yalnız keçmiş təqvim ayı (bax:
    /// MonthlyPayrollGenerationJob) hesablana bilər. Cari/gələcək və köhnə aylar üçün yeni
    /// hesablama yaradıla bilməz.
    /// </summary>
    public static Error PeriodNotClosed(int year, int month, int allowedYear, int allowedMonth) => new(
        "PayrollRun.PeriodNotClosed",
        $"{year}-{month:D2} dövrü hesablanmaq üçün bağlı deyil. Əmək haqqı yalnız ay bitdikdən sonra " +
        $"hesablanır — indi icazəli olan dövr: {allowedYear}-{allowedMonth:D2}.");

    /// <summary>Ödəniş edilmiş hesablama yenidən qurula bilməz (mühasibat qeydləri artıq yaranıb).</summary>
    public static Error HasPayments(Guid payrollRunId) => new(
        "PayrollRun.HasPayments",
        $"PayrollRun {payrollRunId} üzrə artıq ödəniş edilib — rəqəmləri dəyişmək üçün əvvəlcə " +
        "ödənişlərin geri qaytarılması lazımdır.");

    /// <summary>Bonus/düzəliş gəldiyi ay üzrə artıq ödəniş edilib — geriyə doğru tətbiq oluna bilməz.</summary>
    public static Error PeriodAlreadyPaid(int year, int month) => new(
        "PayrollRun.PeriodAlreadyPaid",
        $"{year}-{month:D2} dövrü üçün əmək haqqı artıq ödənilib — bu aya bonus/düzəliş əlavə edilə bilməz " +
        "(yalnız növbəti dövr üçün qeyd yaradıla bilər).");
}
