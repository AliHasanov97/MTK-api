using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.IntegrationEvents.Users;
using MTK.Modules.Payments.Application.Users.Commands.DeactivateUser;

namespace MTK.Modules.Payments.Presentation.Users;

/// <summary>
/// Identity-də istifadəçi silinəndə bu moduldakı User snapshot-unu deaktiv edir
/// (soft — ad hələ də AuditLog-da görünsün deyə snapshot özü silinmir).
/// </summary>
internal sealed class SyncUserOnDeletedIntegrationEventHandler(ISender sender)
    : IntegrationEventHandler<UserDeletedIntegrationEvent>
{
    public override async Task Handle(
        UserDeletedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        var command = new DeactivateUserCommand(integrationEvent.UserId);

        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            throw new InvalidOperationException($"Failed to deactivate user snapshot in Payments module: {result.Error}");
        }
    }
}
