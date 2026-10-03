using FluentValidation;
using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Application.Users.Commands.DeactivateUser;
using MTK.Modules.Identity.IntegrationEvents.Users;

namespace MTK.Modules.Buildings.Presentation.IntegrationEventHandlers.Users;

/// <summary>
/// Identity-də istifadəçi silinəndə bu moduldakı User snapshot-unu deaktiv edir
/// (soft — ad hələ də AuditLog-da görünsün deyə snapshot özü silinmir).
/// </summary>
public sealed class SyncUserOnDeletedIntegrationEventHandler(ISender sender)
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
            throw new ValidationException("Failed to deactivate user snapshot in Buildings module");
        }
    }
}
