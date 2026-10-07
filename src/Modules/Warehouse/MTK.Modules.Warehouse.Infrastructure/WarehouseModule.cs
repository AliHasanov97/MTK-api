using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MTK.Modules.Warehouse.Application.Abstractions.Data;
using MTK.Modules.Warehouse.Application.Nomenclatures.Commands.CreateNomenclature;
using MTK.Modules.Warehouse.Domain.Nomenclatures;
using MTK.Modules.Warehouse.Domain.WarehouseStock;
using MTK.Modules.Warehouse.Domain.WarehouseTransactions;
using MTK.Modules.Warehouse.Infrastructure.Database;
using MTK.Modules.Warehouse.Infrastructure.Repositories;

namespace MTK.Modules.Warehouse.Infrastructure;

public static class WarehouseModule
{
    public static IServiceCollection AddWarehouseModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Application layer
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateNomenclatureCommand).Assembly);
        });

        services.AddValidatorsFromAssembly(
            typeof(CreateNomenclatureCommand).Assembly,
            includeInternalTypes: true);

        // Domain Event Handlers (for ProcessOutboxJob)
        services.AddDomainEventHandlers();

        // Database
        services.AddDbContext<WarehouseDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("Database"),
                npgsqlOptions => npgsqlOptions
                    .MigrationsHistoryTable("__EFMigrationsHistory", "warehouse")));

        // Unit of Work
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<WarehouseDbContext>());

        // Repositories
        services.AddScoped<INomenclatureRepository, NomenclatureRepository>();
        services.AddScoped<IWarehouseTransactionRepository, WarehouseTransactionRepository>();
        services.AddScoped<IWarehouseStockRepository, WarehouseStockRepository>();

        return services;
    }

    private static void AddDomainEventHandlers(this IServiceCollection services)
    {
        // Find all domain event handlers in Application assembly
        Type[] domainEventHandlers = typeof(CreateNomenclatureCommand).Assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(MTK.Common.Application.Messaging.IDomainEventHandler)))
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .ToArray();

        foreach (Type domainEventHandler in domainEventHandlers)
        {
            services.AddScoped(domainEventHandler);
        }
    }
}
