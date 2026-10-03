using MTK.Common.Application.Messaging;

namespace MTK.Modules.Buildings.Application.Users.Commands.UpdateUser;

public sealed record UpdateUserCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    string? IdentityId = null) : ICommand;
