using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.CompensationOrders;

public interface ICompensationOrderRepository : IRepository<CompensationOrder>
{
    Task<CompensationOrder?> GetByCompensationApplicationIdAsync(
        Guid compensationApplicationId,
        CancellationToken cancellationToken = default);

    Task<CompensationOrder?> GetByIdWithEmployeeAndCompanyAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}