using MTK.Common.Presentation.Responses;

namespace MTK.Modules.Hr.Application.AuditLogs.SearchAuditLogs;

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
    // Bu modulun öz User snapshot-undan (Identity-dən sync) — Id+ad. Snapshot
    // tapılmayıbsa (hələ sync olunmayıb) null olur.
    ResponseObjectWithName? User,
    DateTimeOffset Timestamp);
