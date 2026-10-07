using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MTK.Common.Application.EventBus;
using MTK.Common.Infrastructure.Inbox;
using MTK.Common.Infrastructure.Outbox;
using MTK.Modules.Warehouse.Application.Abstractions.Data;
using MTK.Modules.Warehouse.Application.Nomenclatures.Commands.CreateNomenclature;
using MTK.Modules.Warehouse.Domain.AuditLogs;
using MTK.Modules.Warehouse.Domain.Nomenclatures;
using MTK.Modules.Warehouse.Domain.Users;
using MTK.Modules.Warehouse.Domain.WarehouseStock;
using MTK.Modules.Warehouse.Domain.WarehouseTransactions;
using MTK.Modules.Warehouse.Infrastructure.Database;
using MTK.Modules.Warehouse.Infrastructure.Inbox;
using MTK.Modules.Warehouse.Infrastructure.Repositories;
using MTK.Modules.Identity.IntegrationEvents.Users;
using Outbox = MTK.Modules.Warehouse.Infrastructure.Outbox;

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

        // Integration Event Handlers (for ProcessInboxJob)
        services.AddIntegrationEventHandlers();

        // Database
        services.AddHttpContextAccessor();
        services.AddDbContext<WarehouseDbContext>((sp, options) =>
        {
            var outboxInterceptor = sp.GetRequiredService<InsertOutboxMessagesInterceptor>();

            options.UseNpgsql(
                    configuration.GetConnectionString("Database"),
                    npgsqlOptions => npgsqlOptions
                        .MigrationsHistoryTable("__EFMigrationsHistory", "warehouse"))
                .AddInterceptors(outboxInterceptor);
        });

        // Unit of Work
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<WarehouseDbContext>());

        // Repositories
        services.AddScoped<INomenclatureRepository, NomenclatureRepository>();
        services.AddScoped<IWarehouseTransactionRepository, WarehouseTransactionRepository>();
        services.AddScoped<IWarehouseStockRepository, WarehouseStockRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Outbox & Inbox Configuration
        services.Configure<OutboxOptions>(configuration.GetSection("Warehouse:Outbox"));
        services.Configure<InboxOptions>(configuration.GetSection("Warehouse:Inbox"));

        // Quartz Job Configurators
        services.ConfigureOptions<Outbox.ConfigureProcessOutboxJob>();
        services.ConfigureOptions<Inbox.ConfigureProcessInboxJob>();

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

    private static void AddIntegrationEventHandlers(this IServiceCollection services)
    {
        // Find all integration event handlers in Presentation assembly
        Type[] integrationEventHandlers = Presentation.AssemblyReference.Assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IIntegrationEventHandler)))
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .ToArray();

        foreach (Type integrationEventHandler in integrationEventHandlers)
        {
            services.AddScoped(integrationEventHandler);
        }
    }

    public static void ConfigureConsumers(IRegistrationConfigurator registrationConfigurator)
    {
        // Identity-dən gələn user integration event-ləri — bu modulun öz User snapshot-unu
        // (AuditLog.UserId -> oxunan ad/email) sync-də saxlayır.
        registrationConfigurator.AddConsumer<IntegrationEventConsumer<UserCreatedIntegrationEvent>>();
        registrationConfigurator.AddConsumer<IntegrationEventConsumer<UserUpdatedIntegrationEvent>>();
        registrationConfigurator.AddConsumer<IntegrationEventConsumer<UserDeletedIntegrationEvent>>();
    }
}
