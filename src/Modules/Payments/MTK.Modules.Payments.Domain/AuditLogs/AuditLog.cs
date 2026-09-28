using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.AuditLogs;

public sealed class AuditLog : SearchableEntity
{
    private AuditLog() : base() { }

    public string EntityType { get; private set; } = string.Empty;
    public Guid EntityId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }
    public Guid? UserId { get; private set; }
    public DateTimeOffset Timestamp { get; private set; }

    public static AuditLog Create(
        string entityType,
        Guid entityId,
        string action,
        string? oldValues,
        string? newValues,
        Guid? userId)
    {
        var auditLog = new AuditLog
        {
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            OldValues = oldValues,
            NewValues = newValues,
            UserId = userId,
            Timestamp = DateTimeOffset.UtcNow
        };
        auditLog.SetCreatedAt();
        return auditLog;
    }
}