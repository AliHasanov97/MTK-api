using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Groups.DeleteGroup;

public sealed record DeleteGroupCommand(Guid Id) : ICommand;
