using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Common.Infrastructure.EventBus;
using MTK.Common.Infrastructure.Inbox;
using MTK.Common.Infrastructure.Outbox;
using MTK.Modules.Buildings.Application.Apartments.Commands.CreateApartment;
using MTK.Modules.Buildings.Domain.Repositories;
using MTK.Modules.Buildings.Infrastructure.Database;
using MTK.Modules.Buildings.Infrastructure.Repositories;
using MTK.Modules.Buildings.Presentation.IntegrationEventHandlers.Users;
using MTK.Modules.Identity.IntegrationEvents.Users;
using Outbox = MTK.Modules.Buildings.Infrastructure.Outbox;
using Inbox = MTK.Modules.Buildings.Infrastructure.Inbox;

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

        // Integration Event Handlers
        services.AddScoped<IIntegrationEventHandler<UserCreatedIntegrationEvent>,
            UserCreatedIntegrationEventHandler>();
        services.AddScoped<IIntegrationEventHandler<UserUpdatedIntegrationEvent>,
            UserUpdatedIntegrationEventHandler>();
        services.AddScoped<IIntegrationEventHandler<UserRoleChangedIntegrationEvent>,
            UserRoleChangedIntegrationEventHandler>();

        // Outbox & Inbox Configuration
        services.Configure<OutboxOptions>(configuration.GetSection("Buildings:Outbox"));
        services.Configure<InboxOptions>(configuration.GetSection("Buildings:Inbox"));

        // Quartz Job Configurators
        services.ConfigureOptions<Outbox.ConfigureProcessOutboxJob>();
        services.ConfigureOptions<Inbox.ConfigureProcessInboxJob>();

        return services;
    }

    public static void ConfigureConsumers(IRegistrationConfigurator registrationConfigurator)
    {
        // Register integration event consumers from Identity module
        registrationConfigurator.AddConsumer<Inbox.IntegrationEventConsumer<UserCreatedIntegrationEvent>>();
        registrationConfigurator.AddConsumer<Inbox.IntegrationEventConsumer<UserUpdatedIntegrationEvent>>();
        registrationConfigurator.AddConsumer<Inbox.IntegrationEventConsumer<UserRoleChangedIntegrationEvent>>();
    }
}
