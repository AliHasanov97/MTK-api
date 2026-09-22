using FluentValidation;
using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Application.Owners.Commands.CreateOwner;
using MTK.Modules.Identity.IntegrationEvents.Users;

namespace MTK.Modules.Buildings.Presentation.IntegrationEventHandlers.Users;

/// <summary>
/// Identity modulunda User yaradıldıqda Buildings modulunda Owner yaradır
/// Ancaq ApartmentOwner rolu olan istifadəçilər üçün Owner yaradılır
/// </summary>
public sealed class UserCreatedIntegrationEventHandler(ISender sender)
    : IntegrationEventHandler<UserCreatedIntegrationEvent>
{
    public override async Task Handle(
        UserCreatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        // Yalnız ApartmentOwner rolu olan istifadəçilər üçün Owner yaradırıq
        if (integrationEvent.UserRole != "ApartmentOwner")
        {
            return;
        }

        var command = new CreateOwnerCommand(
            integrationEvent.UserId,
            integrationEvent.FirstName,
            integrationEvent.LastName,
            integrationEvent.PhoneNumber,
            integrationEvent.Email);

        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            throw new ValidationException("Failed to create owner in Buildings module");
        }
    }
}
