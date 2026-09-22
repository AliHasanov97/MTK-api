using MTK.Common.Application.Messaging;

namespace MTK.Modules.Buildings.Application.Owners.Commands.UpdateOwnerRole;

public sealed record UpdateOwnerRoleCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string? Email,
    string OldRole,
    string NewRole) : ICommand;
