using MTK.Common.Application.Messaging;

namespace MTK.Modules.Communications.Application.Users.Commands.DeleteUser;

public sealed record DeleteUserCommand(Guid UserId) : ICommand;
