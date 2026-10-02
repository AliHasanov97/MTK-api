namespace MTK.Modules.Payments.Application.AuditLogs.SearchAuditLogs;

public sealed record SearchAuditLogsResponse(
    IReadOnlyCollection<AuditLogDto> AuditLogs,
    int TotalCount,
    int Page,
    int PageSize);

public sealed record AuditLogDto(
    Guid Id,
    string EntityType,
    Guid EntityId,
    string Action,
    string? OldValues,
    string? NewValues,
    Guid? UserId,
    DateTimeOffset Timestamp);
