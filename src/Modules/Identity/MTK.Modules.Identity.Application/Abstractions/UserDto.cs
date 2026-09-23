namespace MTK.Modules.Identity.Application.Abstractions;

public sealed record UserDto(
    string Id,
    string Username,
    string Email,
    string? FirstName,
    string? LastName);
