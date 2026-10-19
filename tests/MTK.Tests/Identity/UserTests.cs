using MTK.Modules.Identity.Domain.Keycloak;
using MTK.Modules.Identity.Domain.Users;
using Xunit;

namespace MTK.Tests.Identity;

public sealed class UserTests
{
    [Fact]
    public void Create_SetsInitialStatusAndPendingSync()
    {
        var user = User.Create("Ali", "Mammadov", "ali@example.com", "+994501234567", roleNames: ["Admin"]);

        Assert.Equal("Ali", user.FirstName);
        Assert.Equal("Mammadov", user.LastName);
        Assert.Equal("ali@example.com", user.Email);
        Assert.Equal(UserStatus.PendingVerification, user.Status);
        Assert.Equal(KeycloakSyncStatus.PendingSync, user.KeycloakSyncStatus);
    }

    [Fact]
    public void MarkSyncFailedThenSynced_ClearsError()
    {
        var user = User.Create("Ali", "Mammadov", "ali@example.com", null);

        user.MarkSyncFailed("Unavailable");
        Assert.Equal(KeycloakSyncStatus.SyncFailed, user.KeycloakSyncStatus);
        Assert.Equal("Unavailable", user.LastSyncError);

        user.MarkSynced();

        Assert.Equal(KeycloakSyncStatus.Synced, user.KeycloakSyncStatus);
        Assert.Null(user.LastSyncError);
    }
}
