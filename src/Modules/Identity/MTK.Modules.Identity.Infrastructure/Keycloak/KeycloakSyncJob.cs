using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MTK.Modules.Identity.Application.Abstractions;
using MTK.Modules.Identity.Domain.Keycloak;
using MTK.Modules.Identity.Infrastructure.Database;

namespace MTK.Modules.Identity.Infrastructure.Keycloak;

/// <summary>
/// Background service that syncs deleted users to Keycloak
/// </summary>
public sealed class KeycloakSyncJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<KeycloakSyncJob> _logger;
    private readonly KeycloakSyncOptions _options;

    public KeycloakSyncJob(
        IServiceScopeFactory scopeFactory,
        ILogger<KeycloakSyncJob> logger,
        IOptions<KeycloakSyncOptions> options)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Keycloak Sync Job started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SyncDeletedUsersAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromMinutes(_options.IntervalInMinutes), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Keycloak Sync Job is stopping");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Keycloak Sync Job");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        _logger.LogInformation("Keycloak Sync Job stopped");
    }

    private async Task SyncDeletedUsersAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        var authService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();

        var deletedUsers = await dbContext.Users
            .IgnoreQueryFilters()
            .Where(u => u.DeletedAt != null)
            .Where(u => u.KeycloakSyncStatus != KeycloakSyncStatus.Synced)
            .OrderBy(u => u.DeletedAt)
            .Take(_options.BatchSize)
            .ToListAsync(cancellationToken);

        if (!deletedUsers.Any())
        {
            return;
        }

        _logger.LogInformation("Syncing {Count} deleted users to Keycloak", deletedUsers.Count);

        var synced = 0;
        var failed = 0;

        foreach (var user in deletedUsers)
        {
            try
            {
                // If no identity ID, mark as synced (nothing to delete in Keycloak)
                if (string.IsNullOrEmpty(user.IdentityId))
                {
                    user.MarkSynced();
                    synced++;
                    continue;
                }

                // Delete from Keycloak
                await authService.DeleteAsync(user.IdentityId, cancellationToken);

                user.MarkSynced();
                synced++;

                _logger.LogInformation("Deleted user {Email} from Keycloak", user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete user {Email} from Keycloak", user.Email);
                user.MarkSyncFailed(ex.Message);
                failed++;
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Keycloak sync completed: {Synced} synced, {Failed} failed", synced, failed);
    }
}
