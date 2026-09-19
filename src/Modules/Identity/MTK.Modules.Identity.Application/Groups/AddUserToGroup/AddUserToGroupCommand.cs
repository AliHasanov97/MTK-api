using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Groups.AddUserToGroup;

public sealed record AddUserToGroupCommand(
    Guid GroupId,
    Guid UserId) : ICommand;
