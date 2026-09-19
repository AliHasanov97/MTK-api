namespace MTK.Modules.Identity.Application.Users.GetUserById;

public sealed record UserDetailResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string Status,
    string? IdentityId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
