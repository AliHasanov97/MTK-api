using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.UnpaidLeaveOrders;

public interface IUnpaidLeaveOrderRepository : IRepository<UnpaidLeaveOrder>
{
    Task<UnpaidLeaveOrder?> GetByUnpaidLeaveApplicationIdAsync(
        Guid unpaidLeaveApplicationId,
        CancellationToken cancellationToken = default);

    Task<UnpaidLeaveOrder?> GetByIdWithEmployeeAndCompanyAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
