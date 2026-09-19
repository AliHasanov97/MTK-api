using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.AuditLogs.SearchAuditLogs;

public sealed record SearchAuditLogsQuery(
    string? EntityType,
    Guid? EntityId,
    string? Action,
    Guid? UserId,
    DateTime? DateFrom,
    DateTime? DateTo,
    int PageNumber,
    int PageSize) : IQuery<SearchAuditLogsResponse>;
