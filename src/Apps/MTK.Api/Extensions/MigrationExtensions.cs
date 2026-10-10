using Microsoft.EntityFrameworkCore;
using MTK.Modules.Buildings.Infrastructure.Database;
using MTK.Modules.Hr.Infrastructure.Database;
using MTK.Modules.Identity.Infrastructure.Database;
using MTK.Modules.Payments.Infrastructure.Database;
using MTK.Modules.Warehouse.Infrastructure.Database;

namespace MTK.Api.Extensions;

public static class MigrationExtensions
{
    /// <summary>Applies pending EF Core migrations for every module on startup.</summary>
    public static async Task ApplyMigrationsAsync(this IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();

        // Apply Identity module migrations
        var identityDbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        await identityDbContext.Database.MigrateAsync();

        // Apply Buildings module migrations
        var buildingsDbContext = scope.ServiceProvider.GetRequiredService<BuildingsDbContext>();
        await buildingsDbContext.Database.MigrateAsync();

        // Apply Payments module migrations
        var paymentsDbContext = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
        await paymentsDbContext.Database.MigrateAsync();

        // Apply Warehouse module migrations
        var warehouseDbContext = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();
        await warehouseDbContext.Database.MigrateAsync();

        // Apply Hr module migrations
        var hrDbContext = scope.ServiceProvider.GetRequiredService<HrDbContext>();
        await hrDbContext.Database.MigrateAsync();

        // TODO: Apply other module migrations here
    }
}
