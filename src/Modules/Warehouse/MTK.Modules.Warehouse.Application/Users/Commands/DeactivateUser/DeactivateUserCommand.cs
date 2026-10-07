using MTK.Common.Application.Messaging;

namespace MTK.Modules.Warehouse.Application.Users.Commands.DeactivateUser;

public sealed record DeactivateUserCommand(Guid UserId) : ICommand;
