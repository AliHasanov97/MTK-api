using FluentValidation;
using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Application.Owners.Commands.UpdateOwner;
using MTK.Modules.Identity.IntegrationEvents.Users;

namespace MTK.Modules.Buildings.Presentation.IntegrationEventHandlers.Users;

/// <summary>
/// Identity modulunda User update edildikdə Buildings modulunda Owner məlumatlarını sync edir
/// </summary>
public sealed class UserUpdatedIntegrationEventHandler(ISender sender)
    : IntegrationEventHandler<UserUpdatedIntegrationEvent>
{
    public override async Task Handle(
        UserUpdatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateOwnerCommand(
            integrationEvent.UserId,
            integrationEvent.FirstName,
            integrationEvent.LastName,
            integrationEvent.PhoneNumber,
            integrationEvent.Email);

        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            throw new ValidationException("Failed to update owner in Buildings module");
        }
    }
}
