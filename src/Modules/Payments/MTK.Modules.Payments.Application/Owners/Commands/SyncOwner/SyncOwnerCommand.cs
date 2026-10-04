using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Owners.Commands.SyncOwner;

public sealed record SyncOwnerCommand(Guid OwnerId, string FullName) : ICommand;
