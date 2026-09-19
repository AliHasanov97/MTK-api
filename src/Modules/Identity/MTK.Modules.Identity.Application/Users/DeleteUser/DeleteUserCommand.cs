using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Users.DeleteUser;

public sealed record DeleteUserCommand(Guid Id) : ICommand;
