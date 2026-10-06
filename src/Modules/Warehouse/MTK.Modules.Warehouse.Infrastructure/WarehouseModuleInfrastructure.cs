using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MTK.Modules.Warehouse.Application.Abstractions;
using MTK.Modules.Warehouse.Domain.Nomenclatures;
using MTK.Modules.Warehouse.Domain.WarehouseStock;
using MTK.Modules.Warehouse.Domain.WarehouseTransactions;
using MTK.Modules.Warehouse.Infrastructure.Database;
using MTK.Modules.Warehouse.Infrastructure.Repositories;

namespace MTK.Modules.Warehouse.Infrastructure;

public static class WarehouseModuleInfrastructure
{
    public static IServiceCollection AddWarehouseInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext
        string? connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<WarehouseDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsqlOptions => npgsqlOptions
                    .MigrationsHistoryTable("__EFMigrationsHistory", "warehouse")));

        services.AddScoped<IWarehouseDbContext>(sp => sp.GetRequiredService<WarehouseDbContext>());

        // Repositories
        services.AddScoped<INomenclatureRepository, NomenclatureRepository>();
        services.AddScoped<IWarehouseTransactionRepository, WarehouseTransactionRepository>();
        services.AddScoped<IWarehouseStockRepository, WarehouseStockRepository>();

        return services;
    }
}
