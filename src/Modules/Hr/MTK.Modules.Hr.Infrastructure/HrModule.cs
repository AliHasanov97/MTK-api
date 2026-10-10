using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MTK.Common.Application.EventBus;
using MTK.Common.Domain.Abstractions;
using MTK.Common.Infrastructure.Inbox;
using MTK.Common.Infrastructure.Outbox;
using MTK.Modules.Hr.Application.Abstractions.Authentication;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.Abstractions.Organization;
using MTK.Modules.Hr.Application.Abstractions.Services.ExportService;
using MTK.Modules.Hr.Application.Timesheets.ExportTimesheet;
using MTK.Modules.Hr.Infrastructure.Export;
using MTK.Modules.Hr.Application.Services;
using MTK.Modules.Hr.Application.VacationApplications.Services;
using MTK.Modules.Hr.Domain.Services;
using MTK.Modules.Hr.Infrastructure.Organization;
using MTK.Modules.Hr.Infrastructure.Authentication;
using MTK.Modules.Hr.Infrastructure.Database;
using MTK.Modules.Hr.Infrastructure.Inbox;
using MTK.Modules.Identity.IntegrationEvents.Users;
using Outbox = MTK.Modules.Hr.Infrastructure.Outbox;

namespace MTK.Modules.Hr.Infrastructure;

public static class HrModule
{
    public static IServiceCollection AddHrModule(
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
        services.AddDbContext<HrDbContext>((sp, options) =>
        {
            var outboxInterceptor = sp.GetRequiredService<InsertOutboxMessagesInterceptor>();

            options.UseNpgsql(
                    configuration.GetConnectionString("Database"),
                    npgsqlOptions => npgsqlOptions
                        .MigrationsHistoryTable("__EFMigrationsHistory", "hr"))
                .AddInterceptors(outboxInterceptor);
        });

        // Unit of Work + current user
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<HrDbContext>());
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IOrganizationInfo, OrganizationInfo>();
        services.AddScoped<IExportService, ExportService>();
        services.AddScoped<ITimesheetExportService, TimesheetExportService>();
        services.AddExportServices();

        // Domain / Application services
        services.AddScoped<WorkingDayCalculatorService>();
        services.AddScoped<ReturnToWorkDateService>();
        services.AddScoped<ILeaveOverlapService, LeaveOverlapService>();
        services.AddScoped<IVacationApplicationAllocationService, VacationApplicationAllocationService>();

        // Repositories: every `I<Name>Repository` of the Domain layer is implemented by exactly one class here
        services.AddRepositories();

        // Outbox & Inbox Configuration
        services.Configure<OutboxOptions>(configuration.GetSection("Hr:Outbox"));
        services.Configure<InboxOptions>(configuration.GetSection("Hr:Inbox"));

        // Quartz Job Configurators
        services.ConfigureOptions<Outbox.ConfigureProcessOutboxJob>();
        services.ConfigureOptions<Inbox.ConfigureProcessInboxJob>();

        return services;
    }

    // Hər sənəd növünün öz PDF/Word generatoru var (…ExportService); ExportService bunların fasadıdır.
    private static void AddExportServices(this IServiceCollection services)
    {
        Type[] generators = typeof(HrModule).Assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && t.Namespace == typeof(ExportService).Namespace)
            .Where(t => t.Name.EndsWith("ExportService") && t != typeof(ExportService) && t != typeof(TimesheetExportService))
            .ToArray();

        foreach (Type generator in generators)
        {
            services.AddScoped(generator);
        }
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        Type[] implementations = typeof(HrModule).Assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && t.Name.EndsWith("Repository"))
            .ToArray();

        foreach (Type implementation in implementations)
        {
            foreach (Type contract in implementation.GetInterfaces()
                         .Where(i => i.Name.EndsWith("Repository") && i.Namespace?.StartsWith("MTK.Modules.Hr.Domain") == true))
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
        // (CreatedById və AuditLog.UserId -> oxunan ad/email üçün).
        registrationConfigurator.AddConsumer<IntegrationEventConsumer<UserCreatedIntegrationEvent>>();
        registrationConfigurator.AddConsumer<IntegrationEventConsumer<UserUpdatedIntegrationEvent>>();
        registrationConfigurator.AddConsumer<IntegrationEventConsumer<UserDeletedIntegrationEvent>>();
    }
}
