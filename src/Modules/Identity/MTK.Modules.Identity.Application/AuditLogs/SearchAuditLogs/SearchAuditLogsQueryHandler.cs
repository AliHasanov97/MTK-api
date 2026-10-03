using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Domain.AuditLogs;

namespace MTK.Modules.Identity.Application.AuditLogs.SearchAuditLogs;

internal sealed class SearchAuditLogsQueryHandler : IQueryHandler<SearchAuditLogsQuery, SearchAuditLogsResponse>
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IMapper _mapper;

    public SearchAuditLogsQueryHandler(IAuditLogRepository auditLogRepository, IMapper mapper)
    {
        _auditLogRepository = auditLogRepository;
        _mapper = mapper;
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

        var items = _mapper.Map<IReadOnlyCollection<AuditLogDto>>(auditLogs);

        var response = new SearchAuditLogsResponse(
            items,
            totalCount,
            request.Page ?? 1,
            request.PageSize ?? 10);

        return response;
    }
}
