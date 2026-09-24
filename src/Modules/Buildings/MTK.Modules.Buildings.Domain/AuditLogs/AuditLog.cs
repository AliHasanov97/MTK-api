using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Buildings.Domain.AuditLogs;

public sealed class AuditLog : Entity
{
    private AuditLog(Guid id, string entityType, Guid entityId, string action, string? oldValues, string? newValues, Guid? userId)
        : base(id)
    {
        EntityType = entityType;
        EntityId = entityId;
        Action = action;
        OldValues = oldValues;
        NewValues = newValues;
        UserId = userId;
        Timestamp = DateTime.UtcNow;
    }

    private AuditLog() : base(Guid.Empty) { }

    public string EntityType { get; private set; } = string.Empty;
    public Guid EntityId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }
    public Guid? UserId { get; private set; }
    public DateTime Timestamp { get; private set; }

    public static AuditLog Create(string entityType, Guid entityId, string action, string? oldValues, string? newValues, Guid? userId)
        => new(Guid.NewGuid(), entityType, entityId, action, oldValues, newValues, userId);
}
