using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Users.RegisterUser;

public sealed record RegisterUserCommand(
    string Email,
    string FirstName,
    string LastName,
    string Password,
    string? PhoneNumber) : ICommand<Guid>;
