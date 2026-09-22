using MTK.Common.Application.Messaging;

namespace MTK.Modules.Buildings.Application.Owners.Commands.CreateOwner;

public sealed record CreateOwnerCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string? Email) : ICommand;
