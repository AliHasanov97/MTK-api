using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Domain.AuditLogs;

namespace MTK.Modules.Identity.Application.AuditLogs.SearchAuditLogs;

internal sealed class SearchAuditLogsQueryHandler : IQueryHandler<SearchAuditLogsQuery, SearchAuditLogsResponse>
{
    private readonly IAuditLogRepository _auditLogRepository;

    public SearchAuditLogsQueryHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<Result<SearchAuditLogsResponse>> Handle(SearchAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var filter = new AuditLogFilter
        {
            EntityType = request.EntityType,
            EntityId = request.EntityId,
            Action = request.Action,
            UserId = request.UserId,
            DateFrom = request.DateFrom,
            DateTo = request.DateTo,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        var auditLogs = await _auditLogRepository.SearchAsync(filter, cancellationToken);

        var auditLogDtos = auditLogs.Select(al => new AuditLogDto(
            al.Id,
            al.EntityType,
            al.EntityId,
            al.Action,
            al.OldValues,
            al.NewValues,
            al.UserId,
            al.Timestamp)).ToList();

        // For simplicity, using the count of returned results as total count
        // In production, you'd want a separate count query
        var totalCount = auditLogDtos.Count;

        var response = new SearchAuditLogsResponse(
            auditLogDtos,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result.Success(response);
    }
}
