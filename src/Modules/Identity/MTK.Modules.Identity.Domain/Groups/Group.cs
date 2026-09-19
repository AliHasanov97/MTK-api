using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Domain.Groups.Events;
using MTK.Modules.Identity.Domain.Keycloak;

namespace MTK.Modules.Identity.Domain.Groups;

public sealed class Group : Entity, IKeycloakSyncable
{
    private Group(
        Guid id,
        Guid keycloakGroupId,
        string name,
        string? description,
        Guid? parentGroupId) : base(id)
    {
        KeycloakGroupId = keycloakGroupId;
        Name = name;
        Description = description;
        ParentGroupId = parentGroupId;
        KeycloakSyncStatus = KeycloakSyncStatus.PendingSync;
    }

    // Private constructor for EF Core
    private Group() : base(Guid.Empty)
    {
    }

    public Guid KeycloakGroupId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Guid? ParentGroupId { get; private set; }
    public string? Description { get; private set; }

    // Keycloak integration
    public KeycloakSyncStatus KeycloakSyncStatus { get; private set; }
    public string? LastSyncError { get; private set; }

    // Timestamps
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    public static Group Create(Guid keycloakGroupId, string name, string? description = null, Guid? parentGroupId = null)
    {
        var group = new Group(
            Guid.NewGuid(),
            keycloakGroupId,
            name,
            description,
            parentGroupId)
        {
            CreatedAt = DateTime.UtcNow
        };

        group.RaiseDomainEvent(new GroupCreatedDomainEvent(
            group.Id,
            group.KeycloakGroupId,
            group.Name,
            group.Description,
            group.ParentGroupId,
            DateTime.UtcNow
        ));

        return group;
    }

    public void Update(string name, string? description = null, Guid? parentGroupId = null)
    {
        Name = name;
        Description = description;
        ParentGroupId = parentGroupId;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new GroupUpdatedDomainEvent(
            Id,
            KeycloakGroupId,
            Name,
            Description,
            DateTime.UtcNow
        ));

        MarkForSync();
    }

    public void Delete()
    {
        DeletedAt = DateTime.UtcNow;

        RaiseDomainEvent(new GroupDeletedDomainEvent(
            Id,
            KeycloakGroupId,
            Name,
            DateTime.UtcNow
        ));

        MarkForSync();
    }

    public void AssignRoles(List<Guid> roleIds)
    {
        RaiseDomainEvent(new GroupRoleAssignedDomainEvent(
            Id,
            roleIds,
            DateTime.UtcNow
        ));
    }

    public void RemoveRoles(List<Guid> roleIds)
    {
        RaiseDomainEvent(new GroupRoleRemovedDomainEvent(
            Id,
            roleIds,
            DateTime.UtcNow
        ));
    }

    public void AssignUser(Guid userId)
    {
        RaiseDomainEvent(new UserGroupAssignedDomainEvent(
            userId,
            Id,
            DateTime.UtcNow
        ));
    }

    public void RemoveUser(Guid userId)
    {
        RaiseDomainEvent(new UserGroupRemovedDomainEvent(
            userId,
            Id,
            DateTime.UtcNow
        ));
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
