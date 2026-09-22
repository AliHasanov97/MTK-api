using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Roles.CreateRealmRole;

public sealed record CreateRealmRoleCommand(
    string Name,
    string? Description) : ICommand<string>;
