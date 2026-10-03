using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.IntegrationEvents.Users;
using MTK.Modules.Payments.Application.Users.Commands.UpdateUser;

namespace MTK.Modules.Payments.Presentation.Users;

/// <summary>Identity-də istifadəçi yenilənəndə bu moduldakı User snapshot-unu sync edir.</summary>
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
            throw new InvalidOperationException($"Failed to sync user snapshot in Payments module: {result.Error}");
        }
    }
}
