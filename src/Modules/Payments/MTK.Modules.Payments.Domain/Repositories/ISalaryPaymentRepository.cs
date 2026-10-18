using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.SalaryPayments;

namespace MTK.Modules.Payments.Domain.Repositories;

/// <summary>
/// Repository interface for SalaryPayment aggregate.
/// </summary>
public interface ISalaryPaymentRepository : IRepository<SalaryPayment>
{
    /// <summary>
    /// SalaryPayment-i Lines ilə birlikdə gətir.
    /// </summary>
    Task<SalaryPayment?> GetByIdWithLinesAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// İşçinin verilmiş dövr üçün maaş ödənişini tap (dublikat yoxlanması üçün).
    /// </summary>
    Task<SalaryPayment?> GetByEmployeeAndPeriodAsync(
        Guid employeeId,
        int year,
        int month,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// İşçinin verilmiş tarix aralığındakı (başlanğıc və son aylar daxil) bütün maaş ödənişlərini gətir.
    /// Orta əmək haqqı hesablaması üçün istifadə olunur.
    /// </summary>
    /// <param name="employeeId">İşçinin ID-si</param>
    /// <param name="fromYear">Başlanğıc ili</param>
    /// <param name="fromMonth">Başlanğıc ayı (1-12)</param>
    /// <param name="toYear">Son il</param>
    /// <param name="toMonth">Son ay (1-12)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Tarix aralığındakı maaş ödənişləri (Status=Paid olan)</returns>
    Task<List<SalaryPayment>> GetByEmployeeAndPeriodRangeAsync(
        Guid employeeId,
        int fromYear,
        int fromMonth,
        int toYear,
        int toMonth,
        CancellationToken cancellationToken = default);
}
