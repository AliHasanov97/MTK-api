using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Domain.AuditLogs;
using MTK.Modules.Hr.Domain.Users;

namespace MTK.Modules.Hr.Application.AuditLogs.SearchAuditLogs;

internal sealed class SearchAuditLogsQueryHandler : IQueryHandler<SearchAuditLogsQuery, SearchAuditLogsResponse>
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IUserRepository _userRepository;

    public SearchAuditLogsQueryHandler(IAuditLogRepository auditLogRepository, IUserRepository userRepository)
    {
        _auditLogRepository = auditLogRepository;
        _userRepository = userRepository;
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

        // UserId -> ad: bu modulun öz User snapshot-undan, frontend ayrıca sorğu atmasın deyə.
        var userIds = auditLogs.Where(al => al.UserId.HasValue).Select(al => al.UserId!.Value).Distinct().ToList();
        var usersById = userIds.Count > 0
            ? (await _userRepository.ListFromIdsAsync(userIds, cancellationToken)).ToDictionary(u => u.Id)
            : [];

        var items = auditLogs.Select(al => new AuditLogDto(
            al.Id,
            al.EntityType,
            al.EntityId,
            al.Action,
            al.OldValues,
            al.NewValues,
            al.UserId.HasValue && usersById.TryGetValue(al.UserId.Value, out var user)
                ? ResponseObjectWithName.Create(user.Id, user.FullName)
                : null,
            al.Timestamp)).ToList();

        return new SearchAuditLogsResponse(
            items,
            totalCount,
            request.Page ?? 1,
            request.PageSize ?? 10);
    }
}
