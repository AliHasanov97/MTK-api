namespace MTK.Modules.Identity.Application.Abstractions;

public interface IUserContext
{
    Guid UserId { get; }
    string IdentityId { get; }
    string FirstName { get; }
    string LastName { get; }
    string Email { get; }
    string? PhoneNumber { get; }
    string Role { get; }
}
