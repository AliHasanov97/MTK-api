using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Warehouse.Domain.AuditLogs;

/// <summary>
/// Anbar audit log-larının oxunması/yazılması üçün repository.
/// </summary>
public interface IAuditLogRepository : IRepository<AuditLog>
{
    /// <summary>
    /// Bir entity üzərində konkret əməliyyatın audit sətri (məs: "bu Nomenklaturanı kim
    /// yaradıb") — əməliyyatın real aktorunu entity-nin özündə saxlamadıqda lazım olur.
    /// </summary>
    Task<AuditLog?> GetForEntityActionAsync(
        string entityType,
        Guid entityId,
        string action,
        CancellationToken cancellationToken = default);
}
