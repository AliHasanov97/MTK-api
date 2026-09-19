namespace MTK.Modules.Identity.Application.Users.SearchUsers;

public sealed record SearchUsersResponse(
    List<UserSearchResult> Users,
    int TotalCount,
    int PageNumber,
    int PageSize);

public sealed record UserSearchResult(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string Status);
