using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Users.RemoveRolesFromUser;

public sealed record RemoveRolesFromUserCommand(
    Guid UserId,
    List<string> RoleNames) : ICommand;
