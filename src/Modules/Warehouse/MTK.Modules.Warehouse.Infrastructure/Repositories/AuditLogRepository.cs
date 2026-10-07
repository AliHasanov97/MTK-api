using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Warehouse.Domain.AuditLogs;
using MTK.Modules.Warehouse.Infrastructure.Database;

namespace MTK.Modules.Warehouse.Infrastructure.Repositories;

internal sealed class AuditLogRepository : SearchableRepository<AuditLog>, IAuditLogRepository
{
    private WarehouseDbContext WarehouseContext => (WarehouseDbContext)Context;

    public AuditLogRepository(WarehouseDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<AuditLog?> GetForEntityActionAsync(
        string entityType,
        Guid entityId,
        string action,
        CancellationToken cancellationToken = default)
    {
        return WarehouseContext.AuditLogs
            .Where(a => a.EntityType == entityType && a.EntityId == entityId && a.Action == action)
            .OrderBy(a => a.Timestamp)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
