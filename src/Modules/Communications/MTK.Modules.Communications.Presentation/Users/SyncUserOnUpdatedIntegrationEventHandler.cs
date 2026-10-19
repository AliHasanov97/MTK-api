using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.IntegrationEvents.Users;
using MTK.Modules.Communications.Application.Users.Commands.UpdateUser;

namespace MTK.Modules.Communications.Presentation.Users;

/// <summary>Identity-də istifadəçi yenilənəndə bu moduldakı User snapshot-unu yeniləyir.</summary>
internal sealed class SyncUserOnUpdatedIntegrationEventHandler(ISender sender)
    : IntegrationEventHandler<UserUpdatedIntegrationEvent>
{
    public override async Task Handle(
        UserUpdatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateUserCommand(
            integrationEvent.UserId,
            integrationEvent.FirstName,
            integrationEvent.LastName,
            integrationEvent.Email,
            integrationEvent.PhoneNumber,
            integrationEvent.IdentityId);

        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            throw new InvalidOperationException($"Failed to sync user snapshot in Communications module: {result.Error}");
        }
    }
}
