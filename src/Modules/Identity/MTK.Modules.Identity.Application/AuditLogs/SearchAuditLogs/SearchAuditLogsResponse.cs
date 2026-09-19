namespace MTK.Modules.Identity.Application.AuditLogs.SearchAuditLogs;

public sealed record SearchAuditLogsResponse(
    List<AuditLogDto> AuditLogs,
    int TotalCount,
    int PageNumber,
    int PageSize);

public sealed record AuditLogDto(
    Guid Id,
    string EntityType,
    Guid EntityId,
    string Action,
    string? OldValues,
    string? NewValues,
    Guid? UserId,
    DateTime Timestamp);
