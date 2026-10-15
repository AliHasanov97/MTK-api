namespace MTK.Modules.Hr.IntegrationEvents.Payroll;

/// <summary>
/// Əmək haqqı hesablamasına təsir edən HR əmrinin növü. Payments tərəfi bu dəyəri
/// öz snapshot-unda saxlayır (bax: Payments.Domain.Payroll.PayrollAdjustmentReason).
/// </summary>
public enum PayrollAdjustmentReason
{
    /// <summary>İstifadə olunmamış məzuniyyət günlərinin pul kompensasiyası (+).</summary>
    VacationCompensation = 1,

    /// <summary>Ödənişsiz məzuniyyət — işlənməmiş günlər maaşdan çıxılır (−).</summary>
    UnpaidLeave = 2,

    /// <summary>Təhsil məzuniyyəti — ödənişlidir, maaşa maliyyə təsiri yoxdur (informasiya).</summary>
    EducationLeave = 3,

    /// <summary>Ödənişli məzuniyyət — maaş saxlanılır, maliyyə təsiri yoxdur (informasiya).</summary>
    Vacation = 4,

    /// <summary>Məzuniyyətdən geri qayıtma — əvvəlki düzəlişin əksi (informasiya).</summary>
    VacationReturn = 5,

    /// <summary>Üzrsüz işdən qalma — gün(lər) maaşdan çıxılır (−).</summary>
    UnexcusedAbsence = 6
}
