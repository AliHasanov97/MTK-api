

namespace MTK.Modules.Warehouse.Application.AuditLogs.SearchAuditLogs;

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
    AuditUserDto? User,
    DateTimeOffset Timestamp);

/// <summary>İstifadəçi Id+ad (JSON-da {id, name} — digər modullardakı ResponseObjectWithName ilə eyni forma).</summary>
public sealed record AuditUserDto(Guid Id, string Name);
