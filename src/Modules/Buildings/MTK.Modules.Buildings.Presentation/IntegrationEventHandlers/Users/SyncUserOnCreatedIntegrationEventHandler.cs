using FluentValidation;
using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Application.Users.Commands.CreateUser;
using MTK.Modules.Identity.IntegrationEvents.Users;

namespace MTK.Modules.Buildings.Presentation.IntegrationEventHandlers.Users;

/// <summary>
/// Identity-də hər hansı istifadəçi yaradıldıqda (rolundan asılı olmayaraq) bu
/// moduldakı User snapshot-unu yaradır — <see cref="UserCreatedIntegrationEventHandler"/>-dən
/// fərqli olaraq (o, yalnız owner rolu üçün Owner yaradır), bu HƏR istifadəçini
/// əhatə edir, AuditLog.UserId-ni ada çevirmək üçün.
/// </summary>
public sealed class SyncUserOnCreatedIntegrationEventHandler(ISender sender)
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
            throw new ValidationException("Failed to sync user snapshot in Buildings module");
        }
    }
}
