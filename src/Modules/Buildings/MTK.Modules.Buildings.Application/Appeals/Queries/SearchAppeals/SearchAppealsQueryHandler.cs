using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Appeals;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.Appeals.Queries.SearchAppeals;

internal sealed class SearchAppealsQueryHandler : IQueryHandler<SearchAppealsQuery, SearchAppealsResponse>
{
    private readonly IAppealRepository _appealRepository;
    private readonly IUserRepository _userRepository;

    public SearchAppealsQueryHandler(
        IAppealRepository appealRepository,
        IUserRepository userRepository)
    {
        _appealRepository = appealRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<SearchAppealsResponse>> Handle(
        SearchAppealsQuery request,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page ?? 1);
        var pageSize = Math.Max(1, request.PageSize ?? 20);

        var appeals = await _appealRepository.SearchAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            page,
            pageSize,
            cancellationToken);

        var totalCount = await _appealRepository.CountAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            cancellationToken);

        // Bütün user ID-lərini toplayaq
        var userIds = appeals.Select(a => a.SubmitterId).Distinct().ToList();

        // Batch lookup - bir query ilə bütün user adlarını alaq
        var userNames = await _userRepository.GetUserNamesByIdsAsync(userIds, cancellationToken);

        // Şərh sayları - Comments naviqasiyası yüklənmədiyi üçün ayrıca sorğu ilə alınır
        var commentCounts = await _appealRepository.GetCommentCountsAsync(
            appeals.Select(a => a.Id).ToList(),
            cancellationToken);

        var response = appeals.Select(a => new AppealListItemResponse(
            a.Id,
            a.Title,
            a.Category,
            a.Status,
            a.Priority,
            a.SubmitterId,
            userNames.TryGetValue(a.SubmitterId, out var submitterName) ? submitterName : "Naməlum istifadəçi",
            a.CreatedAt,
            a.UpdatedAt,
            commentCounts.TryGetValue(a.Id, out var commentsCount) ? commentsCount : 0)).ToList();

        return new SearchAppealsResponse(response, totalCount, page, pageSize);
    }
}
