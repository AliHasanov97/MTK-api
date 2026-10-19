using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MTK.Common.Application.EventBus;
using MTK.Common.Infrastructure.Inbox;
using MTK.Common.Infrastructure.Outbox;
using MTK.Modules.Communications.Application.Abstractions.Authentication;
using MTK.Modules.Communications.Application.Abstractions.Data;
using MTK.Modules.Communications.Infrastructure.Authentication;
using MTK.Modules.Communications.Infrastructure.Database;
using MTK.Modules.Communications.Infrastructure.Inbox;
using Outbox = MTK.Modules.Communications.Infrastructure.Outbox;

namespace MTK.Modules.Communications.Infrastructure;

public static class CommunicationsModule
{
    public static IServiceCollection AddCommunicationsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Application layer
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Application.AssemblyReference.Assembly);
        });

        services.AddValidatorsFromAssembly(
            Application.AssemblyReference.Assembly,
            includeInternalTypes: true);

        // Domain Event Handlers (for ProcessOutboxJob)
        services.AddDomainEventHandlers();

        // Integration Event Handlers (for ProcessInboxJob)
        services.AddIntegrationEventHandlers();

        // Database
        services.AddHttpContextAccessor();
        services.AddDbContext<CommunicationsDbContext>((sp, options) =>
        {
            var outboxInterceptor = sp.GetRequiredService<InsertOutboxMessagesInterceptor>();

            options.UseNpgsql(
                    configuration.GetConnectionString("Database"),
                    npgsqlOptions => npgsqlOptions
                        .MigrationsHistoryTable("__EFMigrationsHistory", "communications"))
                .AddInterceptors(outboxInterceptor);
        });

        // Unit of Work + current user
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<CommunicationsDbContext>());
        services.AddScoped<IUserContext, UserContext>();

        // Repositories
        services.AddRepositories();

        // Outbox & Inbox Configuration
        services.Configure<OutboxOptions>(configuration.GetSection("Communications:Outbox"));
        services.Configure<InboxOptions>(configuration.GetSection("Communications:Inbox"));

        // Quartz Job Configurators
        services.ConfigureOptions<Outbox.ConfigureProcessOutboxJob>();
        services.ConfigureOptions<Inbox.ConfigureProcessInboxJob>();

        return services;
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        Type[] implementations = typeof(CommunicationsModule).Assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && t.Name.EndsWith("Repository"))
            .ToArray();

        foreach (Type implementation in implementations)
        {
            foreach (Type contract in implementation.GetInterfaces()
                         .Where(i => i.Name.EndsWith("Repository") && i.Namespace?.StartsWith("MTK.Modules.Communications.Domain") == true))
            {
                services.AddScoped(contract, implementation);
            }
        }
    }

    private static void AddDomainEventHandlers(this IServiceCollection services)
    {
        Type[] domainEventHandlers = Application.AssemblyReference.Assembly
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
        // Identity-dən gələn user integration event-ləri — bu modulun öz User snapshot-unu sync-də saxlayır
        // (CreatedById -> oxunan ad/email üçün).
        registrationConfigurator.AddConsumer<IntegrationEventConsumer<MTK.Modules.Identity.IntegrationEvents.Users.UserCreatedIntegrationEvent>>();
        registrationConfigurator.AddConsumer<IntegrationEventConsumer<MTK.Modules.Identity.IntegrationEvents.Users.UserUpdatedIntegrationEvent>>();
        registrationConfigurator.AddConsumer<IntegrationEventConsumer<MTK.Modules.Identity.IntegrationEvents.Users.UserDeletedIntegrationEvent>>();
    }
}
