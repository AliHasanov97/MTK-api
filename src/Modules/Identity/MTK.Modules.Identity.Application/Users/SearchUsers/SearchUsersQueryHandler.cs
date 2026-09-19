using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Domain.Users;

namespace MTK.Modules.Identity.Application.Users.SearchUsers;

internal sealed class SearchUsersQueryHandler : IQueryHandler<SearchUsersQuery, SearchUsersResponse>
{
    private readonly IUserRepository _userRepository;

    public SearchUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<SearchUsersResponse>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<User> allUsers = await _userRepository.GetAllAsync(cancellationToken);

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            string searchLower = request.SearchTerm.ToLower();
            allUsers = allUsers.Where(u =>
                u.Email.ToLower().Contains(searchLower) ||
                u.FirstName.ToLower().Contains(searchLower) ||
                u.LastName.ToLower().Contains(searchLower) ||
                (u.PhoneNumber != null && u.PhoneNumber.Contains(searchLower)));
        }

        // Apply sorting
        allUsers = ApplySorting(allUsers, request.SortBy, request.SortDirection);

        int totalCount = allUsers.Count();

        // Apply pagination
        var paginatedUsers = allUsers
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(u => new UserSearchResult(
                u.Id,
                u.Email,
                u.FirstName,
                u.LastName,
                u.PhoneNumber,
                u.Status.ToString()))
            .ToList();

        var response = new SearchUsersResponse(
            paginatedUsers,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result.Success(response);
    }

    private static IEnumerable<User> ApplySorting(IEnumerable<User> users, string? sortBy, string? sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return users.OrderBy(u => u.CreatedAt);
        }

        bool descending = sortDirection?.ToLower() == "desc";

        return sortBy.ToLower() switch
        {
            "email" => descending ? users.OrderByDescending(u => u.Email) : users.OrderBy(u => u.Email),
            "firstname" => descending ? users.OrderByDescending(u => u.FirstName) : users.OrderBy(u => u.FirstName),
            "lastname" => descending ? users.OrderByDescending(u => u.LastName) : users.OrderBy(u => u.LastName),
            "createdat" => descending ? users.OrderByDescending(u => u.CreatedAt) : users.OrderBy(u => u.CreatedAt),
            _ => users.OrderBy(u => u.CreatedAt)
        };
    }
}
