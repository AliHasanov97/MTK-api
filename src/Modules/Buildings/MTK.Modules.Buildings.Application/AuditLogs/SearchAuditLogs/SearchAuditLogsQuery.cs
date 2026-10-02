using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Buildings.Application.AuditLogs.SearchAuditLogs;

public sealed record SearchAuditLogsQuery(
    List<QueryFilter>? Filters,
    SortCriteria? SortCriteria,
    string? SearchTerm,
    int? Page,
    int? PageSize) : IQuery<SearchAuditLogsResponse>;
