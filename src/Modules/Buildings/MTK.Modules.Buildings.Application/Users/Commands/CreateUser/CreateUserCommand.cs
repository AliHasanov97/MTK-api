using MTK.Common.Application.Messaging;

namespace MTK.Modules.Buildings.Application.Users.Commands.CreateUser;

public sealed record CreateUserCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    string? IdentityId = null) : ICommand;
