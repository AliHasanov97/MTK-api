using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.AuditLogs;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

internal sealed class AuditLogRepository : SearchableRepository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }

    public Task<AuditLog?> GetForEntityActionAsync(
        string entityType,
        Guid entityId,
        string action,
        CancellationToken cancellationToken = default)
    {
        return DbItem
            .Where(a => a.EntityType == entityType && a.EntityId == entityId && a.Action == action)
            .OrderBy(a => a.Timestamp)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
