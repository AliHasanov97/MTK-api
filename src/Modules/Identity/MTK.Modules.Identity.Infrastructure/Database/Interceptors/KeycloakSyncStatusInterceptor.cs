using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MTK.Modules.Identity.Domain.Keycloak;

namespace MTK.Modules.Identity.Infrastructure.Database.Interceptors;

/// <summary>
/// Automatically marks entities for Keycloak sync when modified
/// </summary>
public sealed class KeycloakSyncStatusInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
        {
            SetPendingSyncStatus(eventData.Context);
        }

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            SetPendingSyncStatus(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void SetPendingSyncStatus(DbContext context)
    {
        var entries = context.ChangeTracker.Entries<IKeycloakSyncable>()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            // Only mark for sync if not already syncing
            if (entry.Entity.KeycloakSyncStatus != KeycloakSyncStatus.PendingSync)
            {
                entry.Entity.MarkForSync();
            }
        }
    }
}
