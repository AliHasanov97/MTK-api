using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Payroll;

namespace MTK.Modules.Payments.Application.Payroll.Services;

/// <summary>
/// Verilmiş ay üçün əmək haqqı hesablamasını (PayrollRun + Payslip-lər) yaradan əsas servis.
/// Həm manual (CalculatePayrollRunCommand → API) tətəndən, həm də avtomatik Quartz job-dan
/// (hər ayın başlanğıcında əvvəlki ay üçün) çağırılır — MonthlyChargeGenerationJob-un
/// IChargeGenerationService-i necə istifadə etdiyi ilə eyni fəlsəfə.
/// </summary>
public interface IPayrollRunGenerationService
{
    /// <summary>
    /// Verilmiş ay üçün yeni hesablama yaradır. Yalnız keçmiş təqvim ayı üçün mümkündür
    /// (bax: <see cref="PayrollPeriodOptions"/>); əks halda <c>PayrollRun.PeriodNotClosed</c>.
    /// </summary>
    Task<Result<PayrollRunResponse>> GenerateAsync(int year, int month, CancellationToken cancellationToken = default);

    /// <summary>
    /// Mövcud hesablamanı yenidən qurur (HR-dan sonradan gələn bonus/düzəliş və ya manual düzəliş).
    /// Yalnız heç bir qəbz ödənilməmişsə mümkündür; nəticədə run yenidən "Calculated" statusuna düşür.
    /// </summary>
    Task<Result<PayrollRunResponse>> RecalculateAsync(Guid payrollRunId, CancellationToken cancellationToken = default);
}
