using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.VacationPays;

namespace MTK.Modules.Payments.Domain.Repositories;

/// <summary>
/// Repository for VacationPay aggregate.
/// </summary>
public interface IVacationPayRepository : IRepository<VacationPay>
{
    /// <summary>
    /// VacationOrder ID-sinə görə məzuniyyət haqqını tap.
    /// Idempotency yoxlaması üçün (eyni order üçün təkrar yaradılmasın).
    /// </summary>
    Task<VacationPay?> GetByVacationOrderIdAsync(Guid vacationOrderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// İşçinin bütün məzuniyyət haqqlarını gətir (tarixə görə sıralı).
    /// </summary>
    Task<List<VacationPay>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Yaxınlaşan məzuniyyət haqqlarını gətir (DueDate yaxınlaşan və status Pending olan).
    /// </summary>
    /// <param name="fromDate">Başlanğıc tarixi</param>
    /// <param name="toDate">Son tarix</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Due date aralığında olan pending məzuniyyət haqqları</returns>
    Task<List<VacationPay>> GetUpcomingAsync(
        DateOnly fromDate,
        DateOnly toDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Bütün məzuniyyət haqqlarını gətir (filter/pagination ilə).
    /// </summary>
    Task<List<VacationPay>> GetAllAsync(
        VacationPayStatus? statusFilter,
        DateOnly? fromDate,
        DateOnly? toDate,
        string? employeeNameFilter,
        int skip,
        int take,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Məzuniyyət haqqlarının sayını gətir (filter ilə).
    /// </summary>
    Task<int> CountAsync(
        VacationPayStatus? statusFilter,
        DateOnly? fromDate,
        DateOnly? toDate,
        string? employeeNameFilter,
        CancellationToken cancellationToken = default);
}
