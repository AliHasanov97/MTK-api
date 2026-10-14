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
    Task<Result<PayrollRunResponse>> GenerateAsync(int year, int month, CancellationToken cancellationToken = default);
}
