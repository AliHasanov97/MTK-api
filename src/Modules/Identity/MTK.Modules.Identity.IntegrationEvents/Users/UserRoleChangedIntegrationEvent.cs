using MTK.Common.Application.EventBus;

namespace MTK.Modules.Identity.IntegrationEvents.Users;

/// <summary>
/// Identity modulunda User-in rolu dəyişdirildikdə digər module-lara bildiriş göndərmək üçün
/// Məsələn: User -> ApartmentOwner olduqda Buildings modulunda Owner yaradılır
/// ApartmentOwner -> User olduqda Buildings modulunda Owner deaktiv edilir
/// </summary>
public sealed class UserRoleChangedIntegrationEvent : IntegrationEvent
{
    public UserRoleChangedIntegrationEvent(
        Guid integrationEventId,
        DateTime occurredOnUtc,
        Guid userId,
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string oldRole,
        string newRole)
        : base(integrationEventId, occurredOnUtc)
    {
        UserId = userId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        OldRole = oldRole;
        NewRole = newRole;
    }

    public Guid UserId { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string Email { get; }
    public string PhoneNumber { get; }
    public string OldRole { get; }
    public string NewRole { get; }
}
