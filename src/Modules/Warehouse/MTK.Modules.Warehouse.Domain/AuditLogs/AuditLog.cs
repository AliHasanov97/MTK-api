using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Warehouse.Domain.AuditLogs;

/// <summary>
/// Anbar modulunda Entity üzərində edilən hər Create/Update/Delete əməliyyatının
/// izi — WarehouseDbContext.SaveChangesAsync tərəfindən avtomatik yazılır.
/// </summary>
public sealed class AuditLog : SearchableEntity
{
    private AuditLog() : base() { }

    public string EntityType { get; private set; } = string.Empty;
    public Guid EntityId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }
    public Guid? UserId { get; private set; }

    // The actor's business role at the moment of the action (e.g. "building-manager",
    // "accountant") — captured here because Role itself is never persisted anywhere
    // (it's a live Keycloak claim, re-read fresh on every request), so without this
    // snapshot there would be no way to answer "was this done by a Komendant or a
    // Xəzinədar?" after the fact. Null for actions with no resolvable business role.
    public string? ActorRole { get; private set; }

    public DateTimeOffset Timestamp { get; private set; }

    public static AuditLog Create(
        string entityType,
        Guid entityId,
        string action,
        string? oldValues,
        string? newValues,
        Guid? userId,
        string? actorRole = null)
    {
        var auditLog = new AuditLog
        {
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            OldValues = oldValues,
            NewValues = newValues,
            UserId = userId,
            ActorRole = actorRole,
            Timestamp = DateTimeOffset.UtcNow
        };
        auditLog.SetCreatedAt();
        return auditLog;
    }
}
