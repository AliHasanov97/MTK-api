using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.VacationOrders;

public interface IVacationOrderRepository : IRepository<VacationOrder>
{
    /// <summary>
    /// ID ilə vacation order götür
    /// </summary>
    Task<VacationOrder?> GetByIdWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// İşçinin bütün ödənişli məzuniyyət əmrlərini götür
    /// </summary>
    Task<List<VacationOrder>> GetByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);
}
