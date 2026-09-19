using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Groups.RemoveRolesFromGroup;

public sealed record RemoveRolesFromGroupCommand(
    Guid GroupId,
    List<string> RoleNames) : ICommand;
