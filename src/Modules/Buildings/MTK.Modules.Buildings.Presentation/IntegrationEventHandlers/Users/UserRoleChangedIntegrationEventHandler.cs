using FluentValidation;
using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Application.Owners.Commands.UpdateOwnerRole;
using MTK.Modules.Identity.IntegrationEvents.Users;

namespace MTK.Modules.Buildings.Presentation.IntegrationEventHandlers.Users;

/// <summary>
/// Identity modulunda User-in rolu dəyişdikdə Buildings modulunda Owner yaradır və ya deaktiv edir
/// User -> ApartmentOwner: Owner yarat
/// ApartmentOwner -> User: Owner deaktiv et
/// </summary>
public sealed class UserRoleChangedIntegrationEventHandler(ISender sender)
    : IntegrationEventHandler<UserRoleChangedIntegrationEvent>
{
    public override async Task Handle(
        UserRoleChangedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateOwnerRoleCommand(
            integrationEvent.UserId,
            integrationEvent.FirstName,
            integrationEvent.LastName,
            integrationEvent.PhoneNumber,
            integrationEvent.Email,
            integrationEvent.OldRole,
            integrationEvent.NewRole);

        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            throw new ValidationException("Failed to update owner role in Buildings module");
        }
    }
}
