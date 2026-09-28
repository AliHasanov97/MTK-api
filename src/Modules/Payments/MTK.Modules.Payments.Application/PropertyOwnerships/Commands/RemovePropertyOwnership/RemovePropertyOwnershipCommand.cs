using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.PropertyOwnerships.Commands.RemovePropertyOwnership;

public sealed record RemovePropertyOwnershipCommand(Guid PropertyId) : ICommand;
