using MTK.Common.Application.EventBus;

namespace MTK.Modules.Identity.IntegrationEvents.Users;

/// <summary>
/// Identity modulunda User yaradılanda digər module-lara bildiriş göndərmək üçün
/// Buildings module bu event-i consume edib Owner entity yaradır
/// </summary>
public sealed class UserCreatedIntegrationEvent : IntegrationEvent
{
    public UserCreatedIntegrationEvent(
        Guid integrationEventId,
        DateTime occurredOnUtc,
        Guid userId,
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string[] roleNames)
        : base(integrationEventId, occurredOnUtc)
    {
        UserId = userId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        RoleNames = roleNames ?? Array.Empty<string>();
    }

    public Guid UserId { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string Email { get; }
    public string PhoneNumber { get; }
    public string[] RoleNames { get; }

    /// <summary>
    /// Helper method to check if user has a specific role
    /// </summary>
    public bool HasRole(string roleName) =>
        RoleNames.Contains(roleName, StringComparer.OrdinalIgnoreCase);
}
