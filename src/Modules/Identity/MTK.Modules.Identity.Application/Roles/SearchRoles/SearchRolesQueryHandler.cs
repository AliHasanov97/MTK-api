using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Domain.Roles;

namespace MTK.Modules.Identity.Application.Roles.SearchRoles;

internal sealed class SearchRolesQueryHandler : IQueryHandler<SearchRolesQuery, SearchRolesResponse>
{
    private readonly IRoleRepository _roleRepository;

    public SearchRolesQueryHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<Result<SearchRolesResponse>> Handle(SearchRolesQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Role> allRoles = await _roleRepository.GetAllAsync(cancellationToken);

        // Apply search filter
        IEnumerable<Role> filteredRoles = allRoles;
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            string searchLower = request.SearchTerm.ToLower();
            filteredRoles = allRoles.Where(r =>
                r.Name.ToLower().Contains(searchLower) ||
                (r.Description != null && r.Description.ToLower().Contains(searchLower)));
        }

        // Apply sorting
        filteredRoles = ApplySorting(filteredRoles, request.SortBy, request.SortDirection);

        int totalCount = filteredRoles.Count();

        // Apply pagination
        var paginatedRoles = filteredRoles
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new RoleSearchResult(
                r.Id,
                r.Name,
                r.Description,
                r.RoleType.ToString(),
                r.IsActive))
            .ToList();

        var response = new SearchRolesResponse(
            paginatedRoles,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result.Success(response);
    }

    private static IEnumerable<Role> ApplySorting(IEnumerable<Role> roles, string? sortBy, string? sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return roles.OrderBy(r => r.CreatedAt);
        }

        bool descending = sortDirection?.ToLower() == "desc";

        return sortBy.ToLower() switch
        {
            "name" => descending ? roles.OrderByDescending(r => r.Name) : roles.OrderBy(r => r.Name),
            "createdat" => descending ? roles.OrderByDescending(r => r.CreatedAt) : roles.OrderBy(r => r.CreatedAt),
            _ => roles.OrderBy(r => r.CreatedAt)
        };
    }
}
