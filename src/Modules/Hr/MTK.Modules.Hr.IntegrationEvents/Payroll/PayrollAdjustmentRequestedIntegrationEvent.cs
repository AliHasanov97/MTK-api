using MTK.Common.Application.EventBus;

namespace MTK.Modules.Hr.IntegrationEvents.Payroll;

/// <summary>
/// HR-dəki məzuniyyət / ödənişsiz məzuniyyət / təhsil məzuniyyəti / məzuniyyət kompensasiyası /
/// üzrsüz qalma əmrləri əmək haqqı hesablamasına <b>iş saatı</b> əsasında təsir edir.
///
/// Hesablama (istehsalat saatı): saatlıq dərəcə = aylıq baza maaş / ayın iş saatı norması.
/// Payments tərəfi <see cref="Hours"/> (işarəli) və <see cref="NormHours"/> (həmin ay üçün
/// işçinin qrafikinə görə norma) saxlayır; pul = baza maaş × (saat / norma).
///
/// YENİ: <see cref="FixedAmount"/> təmin edilibsə, saat əsaslı hesablama ignore edilir və
/// sabit məbləğ istifadə olunur (məs. kompensasiya üçün orta əmək haqqı hesablaması).
/// </summary>
public sealed class PayrollAdjustmentRequestedIntegrationEvent : IntegrationEvent
{
    public PayrollAdjustmentRequestedIntegrationEvent(
        Guid integrationEventId,
        DateTime occurredOnUtc,
        Guid orderId,
        Guid employeeId,
        PayrollAdjustmentReason reason,
        decimal hours,
        decimal normHours,
        int salaryMonth,
        int salaryYear,
        decimal? fixedAmount = null)
        : base(integrationEventId, occurredOnUtc)
    {
        OrderId = orderId;
        EmployeeId = employeeId;
        Reason = reason;
        Hours = hours;
        NormHours = normHours;
        SalaryMonth = salaryMonth;
        SalaryYear = salaryYear;
        FixedAmount = fixedAmount;
    }

    /// <summary>Hadisəni doğuran HR əmrinin Id-si — Payments tərəfində idempotentlik açarı.</summary>
    public Guid OrderId { get; }

    public Guid EmployeeId { get; }

    public PayrollAdjustmentReason Reason { get; }

    /// <summary>İşarəli iş saatı: müsbət = əlavə ödəniş, mənfi = maaşdan çıxılma.</summary>
    public decimal Hours { get; }

    /// <summary>Həmin ay/dövr üçün işçinin iş saatı norması (baza maaş bunun üzərinə bölünür).</summary>
    public decimal NormHours { get; }

    /// <summary>1-12.</summary>
    public int SalaryMonth { get; }

    public int SalaryYear { get; }

    /// <summary>
    /// Sabit məbləğ (opsional). Əgər təmin edilibsə, Hours/NormHours ignore edilir.
    /// İstifadə: Kompensasiya üçün orta əmək haqqı əsasında hesablama.
    /// </summary>
    public decimal? FixedAmount { get; }
}
