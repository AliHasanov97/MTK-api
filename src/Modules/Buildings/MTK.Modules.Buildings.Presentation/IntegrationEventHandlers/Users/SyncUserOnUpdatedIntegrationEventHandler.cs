using FluentValidation;
using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Application.Users.Commands.UpdateUser;
using MTK.Modules.Identity.IntegrationEvents.Users;

namespace MTK.Modules.Buildings.Presentation.IntegrationEventHandlers.Users;

/// <summary>Identity-də hər hansı istifadəçi yenilənəndə bu moduldakı User snapshot-unu sync edir.</summary>
public sealed class SyncUserOnUpdatedIntegrationEventHandler(ISender sender)
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
            throw new ValidationException("Failed to sync user snapshot in Buildings module");
        }
    }
}
