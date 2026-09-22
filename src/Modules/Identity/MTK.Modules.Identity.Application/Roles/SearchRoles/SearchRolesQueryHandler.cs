using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;

namespace MTK.Modules.Identity.Application.Roles.SearchRoles;

internal sealed class SearchRolesQueryHandler : IQueryHandler<SearchRolesQuery, SearchRolesResponse>
{
    private readonly IAuthenticationService _authenticationService;

    public SearchRolesQueryHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<Result<SearchRolesResponse>> Handle(SearchRolesQuery request, CancellationToken cancellationToken)
    {
        var allRoleNames = await _authenticationService.GetRealmRoleNamesAsync(cancellationToken);

        // Apply search filter
        IEnumerable<string> filteredRoleNames = allRoleNames;
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            string searchLower = request.SearchTerm.ToLower();
            filteredRoleNames = allRoleNames.Where(name => name.ToLower().Contains(searchLower));
        }

        // Apply sorting
        filteredRoleNames = ApplySorting(filteredRoleNames, request.SortBy, request.SortDirection);

        int totalCount = filteredRoleNames.Count();

        // Apply pagination
        var paginatedRoles = filteredRoleNames
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(name => new RoleSearchResult(
                Guid.Empty,
                name,
                string.Empty,
                "Realm",
                true))
            .ToList();

        var response = new SearchRolesResponse(
            paginatedRoles,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result.Success(response);
    }

    private static IEnumerable<string> ApplySorting(IEnumerable<string> roleNames, string? sortBy, string? sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return roleNames.OrderBy(name => name);
        }

        bool descending = sortDirection?.ToLower() == "desc";

        return sortBy.ToLower() switch
        {
            "name" => descending ? roleNames.OrderByDescending(name => name) : roleNames.OrderBy(name => name),
            _ => roleNames.OrderBy(name => name)
        };
    }
}
