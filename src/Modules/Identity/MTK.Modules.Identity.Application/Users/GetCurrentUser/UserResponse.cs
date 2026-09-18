namespace MTK.Modules.Identity.Application.Users.GetCurrentUser;

public sealed record UserResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string Role);
