using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Users.UpdateUser;

public sealed record UpdateUserCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string? PhoneNumber) : ICommand;
