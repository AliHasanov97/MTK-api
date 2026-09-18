namespace MTK.Modules.Identity.Domain.Keycloak;

public interface IKeycloakSyncable
{
    KeycloakSyncStatus KeycloakSyncStatus { get; }
    string? LastSyncError { get; }

    void MarkForSync();
    void MarkSynced();
    void MarkSyncFailed(string error);
}
