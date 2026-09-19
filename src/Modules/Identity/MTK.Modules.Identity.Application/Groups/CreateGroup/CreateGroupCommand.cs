using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Groups.CreateGroup;

public sealed record CreateGroupCommand(
    string Name,
    string? Description,
    Guid? ParentGroupId) : ICommand<Guid>;
