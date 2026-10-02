using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.AuditLogs.SearchAuditLogs;

internal sealed class SearchAuditLogsQueryHandler : IQueryHandler<SearchAuditLogsQuery, SearchAuditLogsResponse>
{
    private readonly IAuditLogRepository _auditLogRepository;

    public SearchAuditLogsQueryHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<Result<SearchAuditLogsResponse>> Handle(
        SearchAuditLogsQuery request,
        CancellationToken cancellationToken)
    {
        var auditLogs = await _auditLogRepository.SearchAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalCount = await _auditLogRepository.CountAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            cancellationToken);

        var items = auditLogs.Select(al => new AuditLogDto(
            al.Id,
            al.EntityType,
            al.EntityId,
            al.Action,
            al.OldValues,
            al.NewValues,
            al.UserId,
            al.Timestamp)).ToList();

        var response = new SearchAuditLogsResponse(
            items,
            totalCount,
            request.Page ?? 1,
            request.PageSize ?? 10);

        return response;
    }
}
