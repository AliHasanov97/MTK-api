using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.VacationBalances;

public interface IVacationBalanceRepository : IRepository<VacationBalance>
{
    /// <summary>Verilmiş tarixi əhatə edən iş ili balansı (WorkYearStart &lt;= date &lt;= WorkYearEnd).</summary>
    Task<VacationBalance?> GetCurrentAsync(Guid employeeId, DateOnly asOfDate, CancellationToken cancellationToken = default);

    /// <summary>Verilmiş iş ili başlanğıcından əvvəlki ən son balans sətri (carry-over üçün).</summary>
    Task<VacationBalance?> GetMostRecentBeforeAsync(Guid employeeId, DateOnly workYearStart, CancellationToken cancellationToken = default);

    /// <summary>Qalan günləri olan ən köhnə iş ili balansı (FIFO - kompensasiya ödəniş üçün).</summary>
    Task<VacationBalance?> GetOldestWithRemainingDaysAsync(Guid employeeId, CancellationToken cancellationToken = default);
}
