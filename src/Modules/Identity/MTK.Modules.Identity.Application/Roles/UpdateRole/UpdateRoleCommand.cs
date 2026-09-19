using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Roles.UpdateRole;

public sealed record UpdateRoleCommand(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive) : ICommand;
