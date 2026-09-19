using MTK.Common.Application.Messaging;
using MTK.Modules.Identity.Domain.Roles;

namespace MTK.Modules.Identity.Application.Roles.CreateRealmRole;

public sealed record CreateRealmRoleCommand(
    string Name,
    string? Description,
    RoleType RoleType) : ICommand<Guid>;
