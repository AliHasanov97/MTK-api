using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Users.AssignRolesToUser;

public sealed record AssignRolesToUserCommand(
    Guid UserId,
    List<string> RoleNames) : ICommand;
