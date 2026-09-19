using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Domain.Keycloak;
using MTK.Modules.Identity.Domain.Roles.Events;

namespace MTK.Modules.Identity.Domain.Roles;

public sealed class Role : Entity, IKeycloakSyncable
{
    private Role(
        Guid id,
        string name,
        RoleType roleType,
        string? description,
        bool isActive) : base(id)
    {
        Name = name;
        RoleType = roleType;
        Description = description;
        IsActive = isActive;
        KeycloakSyncStatus = KeycloakSyncStatus.PendingSync;
    }

    // Private constructor for EF Core
    private Role() : base(Guid.Empty)
    {
    }

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public RoleType RoleType { get; private set; }
    public bool IsActive { get; private set; }

    // Keycloak integration
    public KeycloakSyncStatus KeycloakSyncStatus { get; private set; }
    public string? LastSyncError { get; private set; }

    // Timestamps
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    public static Role Create(string name, RoleType roleType, string? description = null, bool isActive = true)
    {
        var role = new Role(
            Guid.NewGuid(),
            name,
            roleType,
            description,
            isActive)
        {
            CreatedAt = DateTime.UtcNow
        };

        role.RaiseDomainEvent(new RoleCreatedDomainEvent(
            role.Id,
            role.Name,
            role.Description,
            role.RoleType,
            DateTime.UtcNow
        ));

        return role;
    }

    public void Update(string name, string? description = null)
    {
        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new RoleUpdatedDomainEvent(
            Id,
            Name,
            Description,
            RoleType,
            IsActive,
            DateTime.UtcNow
        ));

        MarkForSync();
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new RoleUpdatedDomainEvent(
            Id,
            Name,
            Description,
            RoleType,
            IsActive,
            DateTime.UtcNow
        ));

        MarkForSync();
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new RoleUpdatedDomainEvent(
            Id,
            Name,
            Description,
            RoleType,
            IsActive,
            DateTime.UtcNow
        ));

        MarkForSync();
    }

    public void Delete()
    {
        DeletedAt = DateTime.UtcNow;

        RaiseDomainEvent(new RoleDeletedDomainEvent(
            Id,
            Name,
            DateTime.UtcNow
        ));

        MarkForSync();
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
