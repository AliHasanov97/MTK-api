using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.Payroll;

/// <summary>
/// HR-dəki məzuniyyət / ödənişsiz məzuniyyət / təhsil məzuniyyəti / məzuniyyət kompensasiyası /
/// məzuniyyətdən qayıtma / üzrsüz qalma əmrlərinin bu moduldakı snapshot-u (Id = HR əmrinin Id-si).
///
/// Düzəliş <b>iş saatı</b> əsasındadır: <see cref="Hours"/> işarəlidir (müsbət = əlavə ödəniş,
/// mənfi = maaşdan çıxılma), <see cref="NormHours"/> isə həmin dövr üçün işçinin iş saatı normasıdır.
/// Əmək haqqı hesablamasında pul = baza maaş × (Hours / NormHours) — yəni saatlıq dərəcə
/// (baza maaş / norma) çıxılan/əlavə olunan saata vurulur.
///
/// <see cref="FixedAmount"/> təmin edilibsə, saat əsaslı hesablama ignore edilir və sabit məbləğ
/// istifadə olunur (məs. kompensasiya üçün orta əmək haqqı hesablaması).
/// </summary>
public sealed class PayrollAdjustment : Entity
{
    private PayrollAdjustment(
        Guid id,
        Guid employeeId,
        PayrollAdjustmentReason reason,
        decimal hours,
        decimal normHours,
        int salaryMonth,
        int salaryYear,
        decimal? fixedAmount = null) : base(id)
    {
        EmployeeId = employeeId;
        Reason = reason;
        Hours = hours;
        NormHours = normHours;
        SalaryMonth = salaryMonth;
        SalaryYear = salaryYear;
        FixedAmount = fixedAmount;
    }

    private PayrollAdjustment() : base() { }

    public Guid EmployeeId { get; private set; }

    public PayrollAdjustmentReason Reason { get; private set; }

    /// <summary>İşarəli iş saatı: müsbət = əlavə ödəniş, mənfi = maaşdan çıxılma.</summary>
    public decimal Hours { get; private set; }

    /// <summary>Həmin ay/dövr üçün işçinin iş saatı norması (saatlıq dərəcənin məxrəci).</summary>
    public decimal NormHours { get; private set; }

    /// <summary>1-12.</summary>
    public int SalaryMonth { get; private set; }

    public int SalaryYear { get; private set; }

    /// <summary>
    /// Sabit məbləğ (opsional). Təmin edilibsə, Hours/NormHours əsaslı hesablama ignore edilir.
    /// Kompensasiya üçün orta əmək haqqı əsasında hesablama.
    /// </summary>
    public decimal? FixedAmount { get; private set; }

    public static PayrollAdjustment Create(
        Guid id,
        Guid employeeId,
        PayrollAdjustmentReason reason,
        decimal hours,
        decimal normHours,
        int salaryMonth,
        int salaryYear,
        decimal? fixedAmount = null)
    {
        var adjustment = new PayrollAdjustment(id, employeeId, reason, hours, normHours, salaryMonth, salaryYear, fixedAmount);
        adjustment.SetCreatedAt();
        return adjustment;
    }
}
