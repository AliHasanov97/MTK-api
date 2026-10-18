using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.Payroll;

namespace MTK.Modules.Payments.Application.Payroll.Commands.RecalculatePayrollRun;

/// <summary>
/// Mövcud hesablamanı yenidən qurur (HR-dan sonradan gələn bonus/düzəlişləri də daxil etmək üçün).
/// Yalnız heç bir qəbz ödənilməmişsə mümkündür — nəticədə run yenidən "Calculated" olur.
/// </summary>
public sealed record RecalculatePayrollRunCommand(Guid PayrollRunId) : ICommand<PayrollRunResponse>;
