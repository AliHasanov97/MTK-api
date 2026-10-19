using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.IntegrationEvents.Users;
using MTK.Modules.Communications.Application.Users.Commands.DeleteUser;

namespace MTK.Modules.Communications.Presentation.Users;

/// <summary>Identity-də istifadəçi silinəndə bu moduldakı User snapshot-unu deaktiv edir.</summary>
internal sealed class SyncUserOnDeletedIntegrationEventHandler(ISender sender)
    : IntegrationEventHandler<UserDeletedIntegrationEvent>
{
    public override async Task Handle(
        UserDeletedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteUserCommand(integrationEvent.UserId);

        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            throw new InvalidOperationException($"Failed to sync user snapshot in Communications module: {result.Error}");
        }
    }
}
