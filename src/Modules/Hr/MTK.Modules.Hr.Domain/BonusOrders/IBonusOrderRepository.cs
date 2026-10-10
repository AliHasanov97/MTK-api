using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.BonusOrders;

public interface IBonusOrderRepository : IRepository<BonusOrder>
{
    Task<BonusOrder?> GetByIdForExportAsync(Guid id, CancellationToken cancellationToken = default);
}
