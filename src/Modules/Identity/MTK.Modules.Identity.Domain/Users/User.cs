using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Domain.Keycloak;

namespace MTK.Modules.Identity.Domain.Users;

public sealed class User : Entity, IKeycloakSyncable
{
    private User(
        Guid id,
        string firstName,
        string lastName,
        string email,
        string? phoneNumber,
        UserStatus status) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Status = status;
        KeycloakSyncStatus = KeycloakSyncStatus.PendingSync;
    }

    // Private constructor for EF Core
    private User() : base(Guid.Empty)
    {
    }

    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public UserStatus Status { get; private set; }

    // Keycloak integration
    public string? IdentityId { get; private set; }
    public KeycloakSyncStatus KeycloakSyncStatus { get; private set; }
    public string? LastSyncError { get; private set; }

    // Timestamps
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    public static User Create(
        string firstName,
        string lastName,
        string email,
        string? phoneNumber,
        UserStatus status = UserStatus.PendingVerification,
        string[]? roleNames = null)
    {
        var user = new User(
            Guid.NewGuid(),
            firstName,
            lastName,
            email,
            phoneNumber,
            status)
        {
            CreatedAt = DateTime.UtcNow
        };

        user.RaiseDomainEvent(new UserCreatedDomainEvent(
            user.Id,
            roleNames ?? Array.Empty<string>()));

        return user;
    }

    public void Update(string firstName, string lastName, string? phoneNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new Users.Events.UserUpdatedDomainEvent(Id, DateTime.UtcNow));
        MarkForSync();
    }

    public void UpdateEmail(string email)
    {
        Email = email;
        UpdatedAt = DateTime.UtcNow;

        MarkForSync();
    }

    public void UpdateStatus(UserStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        Status = UserStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Status = UserStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Block()
    {
        Status = UserStatus.Blocked;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        DeletedAt = DateTime.UtcNow;

        RaiseDomainEvent(new Users.Events.UserDeletedDomainEvent(Id, DateTime.UtcNow));
        MarkForSync();
    }

    public void SetIdentityId(string identityId)
    {
        IdentityId = identityId;
    }

    // IKeycloakSyncable implementation
    public void MarkForSync()
    {
        KeycloakSyncStatus = KeycloakSyncStatus.PendingSync;
        LastSyncError = null;
    }

    public void MarkSynced()
    {
        KeycloakSyncStatus = KeycloakSyncStatus.Synced;
        LastSyncError = null;
    }

    public void MarkSyncFailed(string error)
    {
        KeycloakSyncStatus = KeycloakSyncStatus.SyncFailed;
        LastSyncError = error;
    }
}
