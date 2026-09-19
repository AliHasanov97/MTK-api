using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Groups.RemoveUserFromGroup;

public sealed record RemoveUserFromGroupCommand(
    Guid GroupId,
    Guid UserId) : ICommand;
