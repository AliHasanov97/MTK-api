using MTK.Common.Application.Messaging;

namespace MTK.Modules.Buildings.Application.Owners.Commands.UpdateOwner;

public sealed record UpdateOwnerCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string? Email) : ICommand;
