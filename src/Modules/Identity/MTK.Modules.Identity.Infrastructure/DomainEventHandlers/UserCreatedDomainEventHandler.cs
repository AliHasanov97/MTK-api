using MediatR;
using MTK.Common.Application.EventBus;
using MTK.Modules.Identity.Domain.Users;
using MTK.Modules.Identity.IntegrationEvents.Users;

namespace MTK.Modules.Identity.Infrastructure.DomainEventHandlers;

/// <summary>
/// Handles UserCreatedDomainEvent and publishes UserCreatedIntegrationEvent
/// to notify other modules (like Buildings) about the new user
/// </summary>
internal sealed class UserCreatedDomainEventHandler : INotificationHandler<UserCreatedDomainEvent>
{
    private readonly IUserRepository _userRepository;
    private readonly IEventBus _eventBus;

    public UserCreatedDomainEventHandler(
        IUserRepository userRepository,
        IEventBus eventBus)
    {
        _userRepository = userRepository;
        _eventBus = eventBus;
    }

    public async Task Handle(
        UserCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken)
    {
        // Get user details from repository
        var user = await _userRepository.GetByIdAsync(domainEvent.UserId, cancellationToken);

        if (user is null)
        {
            // User should exist, but handle gracefully
            return;
        }

        // Create and publish integration event
        var integrationEvent = new UserCreatedIntegrationEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.PhoneNumber ?? string.Empty,
            domainEvent.RoleNames);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
