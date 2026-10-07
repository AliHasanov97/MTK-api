using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.IntegrationEvents.Users;
using MTK.Modules.Warehouse.Application.Users.Commands.CreateUser;

namespace MTK.Modules.Warehouse.Presentation.Users;

/// <summary>Identity-də istifadəçi yaradılanda bu moduldakı User snapshot-unu yaradır.</summary>
internal sealed class SyncUserOnCreatedIntegrationEventHandler(ISender sender)
    : IntegrationEventHandler<UserCreatedIntegrationEvent>
{
    public override async Task Handle(
        UserCreatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateUserCommand(
            integrationEvent.UserId,
            integrationEvent.FirstName,
            integrationEvent.LastName,
            integrationEvent.Email,
            integrationEvent.PhoneNumber,
            integrationEvent.IdentityId);

        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            throw new InvalidOperationException($"Failed to sync user snapshot in Warehouse module: {result.Error}");
        }
    }
}
