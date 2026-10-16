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
}
