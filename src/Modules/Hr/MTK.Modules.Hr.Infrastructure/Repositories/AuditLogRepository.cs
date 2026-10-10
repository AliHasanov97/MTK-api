using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.AuditLogs;
using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class AuditLogRepository : SearchableRepository<AuditLog>, IAuditLogRepository
{
    private HrDbContext HrContext => (HrDbContext)Context;

    public AuditLogRepository(HrDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<AuditLog?> GetForEntityActionAsync(
        string entityType,
        Guid entityId,
        string action,
        CancellationToken cancellationToken = default)
    {
        return HrContext.AuditLogs
            .Where(a => a.EntityType == entityType && a.EntityId == entityId && a.Action == action)
            .OrderBy(a => a.Timestamp)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
