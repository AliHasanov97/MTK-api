using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Domain.Groups;

namespace MTK.Modules.Identity.Application.Groups.SearchGroups;

internal sealed class SearchGroupsQueryHandler : IQueryHandler<SearchGroupsQuery, SearchGroupsResponse>
{
    private readonly IGroupRepository _groupRepository;

    public SearchGroupsQueryHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<Result<SearchGroupsResponse>> Handle(SearchGroupsQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Group> allGroups = await _groupRepository.GetAllAsync(cancellationToken);

        // Apply search filter
        IEnumerable<Group> filteredGroups = allGroups;
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            string searchLower = request.SearchTerm.ToLower();
            filteredGroups = allGroups.Where(g =>
                g.Name.ToLower().Contains(searchLower) ||
                (g.Description != null && g.Description.ToLower().Contains(searchLower)));
        }

        // Apply sorting
        filteredGroups = ApplySorting(filteredGroups, request.SortBy, request.SortDirection);

        int totalCount = filteredGroups.Count();

        // Apply pagination
        var paginatedGroups = filteredGroups
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(g => new GroupSearchResult(
                g.Id,
                g.KeycloakGroupId,
                g.Name,
                g.Description,
                g.ParentGroupId))
            .ToList();

        var response = new SearchGroupsResponse(
            paginatedGroups,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result.Success(response);
    }

    private static IEnumerable<Group> ApplySorting(IEnumerable<Group> groups, string? sortBy, string? sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return groups.OrderBy(g => g.CreatedAt);
        }

        bool descending = sortDirection?.ToLower() == "desc";

        return sortBy.ToLower() switch
        {
            "name" => descending ? groups.OrderByDescending(g => g.Name) : groups.OrderBy(g => g.Name),
            "createdat" => descending ? groups.OrderByDescending(g => g.CreatedAt) : groups.OrderBy(g => g.CreatedAt),
            _ => groups.OrderBy(g => g.CreatedAt)
        };
    }
}
