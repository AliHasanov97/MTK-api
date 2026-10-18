using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Payroll;

namespace MTK.Modules.Payments.Domain.Repositories;

/// <summary>
/// Orta əmək haqqı hesablaması üçün Payslip məlumatları (tarix ilə birlikdə).
/// Domain layer-də DTO yaratmamaq üçün burada yerləşdirilir.
/// </summary>
public sealed record PayslipWithPeriodDto(
    int Year,
    int Month,
    decimal BaseSalary,
    decimal BonusAmount,
    decimal AdjustmentAmount,
    decimal GrossSalary);

public interface IPayrollRunRepository : IRepository<PayrollRun>
{
    /// <summary>Qəbzləri ilə birlikdə bir PayrollRun.</summary>
    Task<PayrollRun?> GetWithPayslipsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Verilmiş ay/il üçün mövcud PayrollRun (təkrar hesablamanın qarşısını almaq üçün).</summary>
    Task<PayrollRun?> GetByPeriodAsync(int year, int month, CancellationToken cancellationToken = default);

    /// <summary>Verilmiş ay/il üçün mövcud PayrollRun, qəbzləri ilə birlikdə (UI-da baxış üçün).</summary>
    Task<PayrollRun?> GetByPeriodWithPayslipsAsync(int year, int month, CancellationToken cancellationToken = default);

    /// <summary>
    /// İşçinin verilmiş tarix aralığındakı (başlanğıc və son aylar daxil) bütün payslip-lərini gətir.
    /// Orta əmək haqqı hesablaması üçün istifadə olunur.
    /// </summary>
    /// <param name="employeeId">İşçinin ID-si</param>
    /// <param name="fromYear">Başlanğıc ili</param>
    /// <param name="fromMonth">Başlanğıc ayı (1-12)</param>
    /// <param name="toYear">Son il</param>
    /// <param name="toMonth">Son ay (1-12)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Tarix aralığındakı Payslip-lər (tarix ilə birlikdə)</returns>
    Task<List<PayslipWithPeriodDto>> GetPayslipsByEmployeeAndPeriodRangeAsync(
        Guid employeeId,
        int fromYear,
        int fromMonth,
        int toYear,
        int toMonth,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// İşçinin bütün payslip-lərini gətir (bütün PayrollRun-lardan).
    /// UI-da işçinin ödəniş tarixçəsini göstərmək üçün.
    /// </summary>
    Task<List<PayrollRun>> GetAllPayrollRunsWithPayslipsByEmployeeAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Bütün PayrollRun-ları gətir (qəbzlər olmadan - yalnız əsas məlumatlar).
    /// UI-da payroll tarixçəsini göstərmək üçün.
    /// </summary>
    Task<List<PayrollRun>> GetAllAsync(CancellationToken cancellationToken = default);
}
