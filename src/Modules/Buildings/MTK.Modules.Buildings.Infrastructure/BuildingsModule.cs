using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MTK.Common.Application.EventBus;
using MTK.Common.Infrastructure.EventBus;
using IUnitOfWork = MTK.Modules.Buildings.Application.Abstractions.Data.IUnitOfWork;
using MTK.Common.Infrastructure.Inbox;
using MTK.Common.Infrastructure.Outbox;
using MTK.Modules.Buildings.Application.Apartments.Commands.CreateApartment;
using MTK.Modules.Buildings.Domain.Repositories;
using MTK.Modules.Buildings.Infrastructure.Database;
using MTK.Modules.Buildings.Infrastructure.Inbox;
using MTK.Modules.Buildings.Infrastructure.Repositories;
using MTK.Modules.Buildings.Presentation.IntegrationEventHandlers.Users;
using MTK.Modules.Identity.IntegrationEvents.Users;
using Outbox = MTK.Modules.Buildings.Infrastructure.Outbox;

namespace MTK.Modules.Buildings.Infrastructure;

public static class BuildingsModule
{
    public static IServiceCollection AddBuildingsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Application layer
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateApartmentCommand).Assembly);
        });

        services.AddValidatorsFromAssembly(
            typeof(CreateApartmentCommand).Assembly,
            includeInternalTypes: true);

        // Integration Event Handlers (for ProcessInboxJob)
        services.AddIntegrationEventHandlers();

        // Database
        services.AddDbContext<BuildingsDbContext>((sp, options) =>
        {
            var outboxInterceptor = sp.GetRequiredService<InsertOutboxMessagesInterceptor>();

            options.UseNpgsql(
                    configuration.GetConnectionString("Database"),
                    npgsqlOptions => npgsqlOptions.MigrationsHistoryTable(
                        "__EFMigrationsHistory",
                        "buildings"))
                .AddInterceptors(outboxInterceptor);
        });

        // Unit of Work
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<BuildingsDbContext>());

        // Repositories
        services.AddScoped<IBuildingRepository, BuildingRepository>();
        services.AddScoped<IApartmentRepository, ApartmentRepository>();
        services.AddScoped<IOwnerRepository, OwnerRepository>();
        services.AddScoped<IGarageRepository, GarageRepository>();
        services.AddScoped<IOwnershipHistoryRepository, OwnershipHistoryRepository>();

        // Outbox & Inbox Configuration
        services.Configure<OutboxOptions>(configuration.GetSection("Buildings:Outbox"));
        services.Configure<InboxOptions>(configuration.GetSection("Buildings:Inbox"));

        // Quartz Job Configurators
        services.ConfigureOptions<Outbox.ConfigureProcessOutboxJob>();
        services.ConfigureOptions<Inbox.ConfigureProcessInboxJob>();

        return services;
    }

    private static void AddIntegrationEventHandlers(this IServiceCollection services)
    {
        // Find all integration event handlers in Presentation assembly
        Type[] integrationEventHandlers = Presentation.AssemblyReference.Assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(MTK.Common.Application.EventBus.IIntegrationEventHandler)))
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .ToArray();

        foreach (Type integrationEventHandler in integrationEventHandlers)
        {
            services.AddScoped(integrationEventHandler);
        }
    }

    public static void ConfigureConsumers(IRegistrationConfigurator registrationConfigurator)
    {
        // Register integration event consumers from Identity module
        registrationConfigurator.AddConsumer<IntegrationEventConsumer<UserCreatedIntegrationEvent>>();
        registrationConfigurator.AddConsumer<IntegrationEventConsumer<UserUpdatedIntegrationEvent>>();
        registrationConfigurator.AddConsumer<IntegrationEventConsumer<UserRoleChangedIntegrationEvent>>();
    }
}
