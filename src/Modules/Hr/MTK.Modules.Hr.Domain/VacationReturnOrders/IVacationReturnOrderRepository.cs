using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.VacationReturnOrders;

public interface IVacationReturnOrderRepository : IRepository<VacationReturnOrder>
{
    /// <summary>
    /// ID ilə vacation return order götür (Employee və Company daxil)
    /// </summary>
    Task<VacationReturnOrder?> GetByIdWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// İşçinin bütün məzuniyyətdən geri qayıtma əmrlərini götür
    /// </summary>
    Task<List<VacationReturnOrder>> GetByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);
}