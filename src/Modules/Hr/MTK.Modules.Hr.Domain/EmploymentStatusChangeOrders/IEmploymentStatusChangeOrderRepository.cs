using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.EmploymentStatusChangeOrders;

public interface IEmploymentStatusChangeOrderRepository : IRepository<EmploymentStatusChangeOrder>
{
    Task<EmploymentStatusChangeOrder?> GetByIdForExportAsync(Guid id, CancellationToken cancellationToken = default);
}