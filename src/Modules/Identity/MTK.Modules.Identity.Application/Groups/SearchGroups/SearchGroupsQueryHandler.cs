using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;

namespace MTK.Modules.Identity.Application.Groups.SearchGroups;

internal sealed class SearchGroupsQueryHandler : IQueryHandler<SearchGroupsQuery, SearchGroupsResponse>
{
    private readonly IAuthenticationService _authenticationService;

    public SearchGroupsQueryHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<Result<SearchGroupsResponse>> Handle(SearchGroupsQuery request, CancellationToken cancellationToken)
    {
        List<GroupDto> allGroups = await _authenticationService.SearchGroupsAsync(request.SearchTerm, cancellationToken);

        int totalCount = allGroups.Count;

        // Apply pagination
        List<GroupDto> paginatedGroups = allGroups
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // Map to response
        var groupResults = paginatedGroups.Select(g => new GroupSearchResult(
            g.Id,
            g.Name,
            g.Description)).ToList();

        var response = new SearchGroupsResponse(
            groupResults,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result.Success(response);
    }
}
