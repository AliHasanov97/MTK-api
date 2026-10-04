using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.AuditLogs;

namespace MTK.Modules.Payments.Domain.Repositories;

public interface IAuditLogRepository : IRepository<AuditLog>
{
    /// <summary>
    /// The audit row for a single lifecycle action on one entity (e.g. "who created
    /// this Payment") — used to recover the real actor of a past operation when that
    /// actor isn't stored on the entity itself (Payment has no CreatedByUserId).
    /// </summary>
    Task<AuditLog?> GetForEntityActionAsync(
        string entityType,
        Guid entityId,
        string action,
        CancellationToken cancellationToken = default);
}
