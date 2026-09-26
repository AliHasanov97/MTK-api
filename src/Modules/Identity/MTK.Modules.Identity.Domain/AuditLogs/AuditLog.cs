using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Identity.Domain.AuditLogs;

public sealed class AuditLog : SearchableEntity
{
    private AuditLog(
        Guid id,
        string entityType,
        Guid entityId,
        string action,
        string? oldValues,
        string? newValues,
        Guid? userId) : base(id)
    {
        EntityType = entityType;
        EntityId = entityId;
        Action = action;
        OldValues = oldValues;
        NewValues = newValues;
        UserId = userId;
        Timestamp = DateTimeOffset.UtcNow;
    }

    // Private constructor for EF Core
    private AuditLog() : base()
    {
    }

    public string EntityType { get; private set; } = string.Empty;
    public Guid EntityId { get; private set; }
    public string Action { get; private set; } = string.Empty; // Created, Updated, Deleted
    public string? OldValues { get; private set; } // JSON
    public string? NewValues { get; private set; } // JSON
    public Guid? UserId { get; private set; }
    public DateTimeOffset Timestamp { get; private set; }

    public static AuditLog Create(
        string entityType,
        Guid entityId,
        string action,
        string? oldValues = null,
        string? newValues = null,
        Guid? userId = null)
    {
        var auditLog = new AuditLog(
            Guid.NewGuid(),
            entityType,
            entityId,
            action,
            oldValues,
            newValues,
            userId);

        auditLog.SetCreatedAt();

        return auditLog;
    }
}
