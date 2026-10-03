using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Users.Commands.DeactivateUser;

public sealed record DeactivateUserCommand(Guid UserId) : ICommand;
