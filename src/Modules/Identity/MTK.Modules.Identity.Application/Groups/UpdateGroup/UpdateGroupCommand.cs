using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Groups.UpdateGroup;

public sealed record UpdateGroupCommand(
    Guid Id,
    string Name,
    string? Description,
    Guid? ParentGroupId) : ICommand;
