using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Roles.DeleteRole;

public sealed record DeleteRoleCommand(string RoleName) : ICommand;
