using MTK.Common.Application.EventBus;

namespace MTK.Modules.Identity.IntegrationEvents.Users;

/// <summary>
/// Identity modulunda User update edildikdə digər module-lara bildiriş göndərmək üçün
/// Buildings module bu event-i consume edib Owner məlumatlarını sync edir
/// </summary>
public sealed class UserUpdatedIntegrationEvent : IntegrationEvent
{
    public UserUpdatedIntegrationEvent(
        Guid integrationEventId,
        DateTime occurredOnUtc,
        Guid userId,
        string firstName,
        string lastName,
        string email,
        string phoneNumber)
        : base(integrationEventId, occurredOnUtc)
    {
        UserId = userId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
    }

    public Guid UserId { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string Email { get; }
    public string PhoneNumber { get; }
}
