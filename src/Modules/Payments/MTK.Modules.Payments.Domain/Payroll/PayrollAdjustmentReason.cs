namespace MTK.Modules.Payments.Domain.Payroll;

/// <summary>
/// Əmək haqqı düzəlişini doğuran HR əmrinin növü. Dəyərlər
/// <c>MTK.Modules.Hr.IntegrationEvents.Payroll.PayrollAdjustmentReason</c> ilə
/// uyğundur (integration event handler dəyəri birbaşa bu tipə çevirir).
/// </summary>
public enum PayrollAdjustmentReason
{
    VacationCompensation = 1,
    UnpaidLeave = 2,
    EducationLeave = 3,
    Vacation = 4,
    VacationReturn = 5,
    UnexcusedAbsence = 6
}
