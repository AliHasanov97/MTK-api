using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Groups.AssignRolesToGroup;

public sealed record AssignRolesToGroupCommand(
    Guid GroupId,
    List<string> RoleNames) : ICommand;
