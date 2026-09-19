namespace MTK.Modules.Identity.Application.Groups.GetGroupUsers;

public sealed record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber);
