using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MTK.Common.Application.Authorization;
using MTK.Common.Domain.Abstractions;
using MTK.Common.Infrastructure.Inbox;
using MTK.Common.Infrastructure.Outbox;
using MTK.Modules.Identity.Application.Abstractions;
using MTK.Modules.Identity.Domain.Users;
using MTK.Modules.Identity.Domain.Groups;
using MTK.Modules.Identity.Domain.Roles;
using MTK.Modules.Identity.Domain.AuditLogs;
using MTK.Modules.Identity.Infrastructure.Authentication;
using MTK.Modules.Identity.Infrastructure.Database;
using MTK.Modules.Identity.Infrastructure.Database.Interceptors;
using MTK.Modules.Identity.Infrastructure.Keycloak;
using MTK.Modules.Identity.Infrastructure.Repositories;
using AuthService = MTK.Modules.Identity.Infrastructure.Authentication.AuthenticationService;

namespace MTK.Modules.Identity.Infrastructure;

public static class IdentityModule
{
    public static IServiceCollection AddIdentityModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Application layer
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(IUserContext).Assembly);
        });

        services.AddValidatorsFromAssembly(typeof(IUserContext).Assembly, includeInternalTypes: true);

        // Database
        services.AddSingleton<ISaveChangesInterceptor, KeycloakSyncStatusInterceptor>();

        services.AddDbContext<IdentityDbContext>((sp, options) =>
        {
            var keycloakInterceptor = sp.GetRequiredService<ISaveChangesInterceptor>();
            var outboxInterceptor = sp.GetRequiredService<InsertOutboxMessagesInterceptor>();

            options.UseNpgsql(configuration.GetConnectionString("Database"))
                .AddInterceptors(keycloakInterceptor, outboxInterceptor);
        });

        // Unit of Work
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<IdentityDbContext>());

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();

        // Authentication & Authorization
        services.AddAuthenticationAndAuthorization(configuration);

        // Outbox & Inbox Configuration
        services.Configure<OutboxOptions>(configuration.GetSection("Identity:Outbox"));
        services.Configure<InboxOptions>(configuration.GetSection("Identity:Inbox"));

        // Quartz Job Configurators
        services.ConfigureOptions<Outbox.ConfigureProcessOutboxJob>();
        services.ConfigureOptions<Inbox.ConfigureProcessInboxJob>();

        // Keycloak Sync
        services.Configure<KeycloakSyncOptions>(configuration.GetSection("Identity:KeycloakSync"));

        return services;
    }

    private static void AddAuthenticationAndAuthorization(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure JWT Bearer
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.AddAuthorization();

        // Configure options
        services.Configure<Authentication.AuthenticationOptions>(configuration.GetSection("Authentication"));
        services.Configure<KeycloakOptions>(configuration.GetSection("Keycloak"));

        services.ConfigureOptions<JwtBearerOptionsSetup>();

        // Claims transformation
        services.AddScoped<IClaimsTransformation, KeycloakRoleTransformer>();

        // Dynamic policy provider
        services.AddSingleton<IAuthorizationPolicyProvider, DynamicRolePolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, KeycloakRoleAuthorizationHandler>();

        // HTTP clients for Keycloak
        services.AddTransient<AdminAuthorizationDelegatingHandler>();

        services.AddHttpClient<Application.Abstractions.IAuthenticationService, AuthService>((sp, httpClient) =>
        {
            var keycloakOptions = configuration.GetSection("Keycloak").Get<KeycloakOptions>() ??
                throw new InvalidOperationException("Keycloak configuration is missing");

            httpClient.BaseAddress = new Uri(keycloakOptions.AdminUrl);
        })
        .AddHttpMessageHandler<AdminAuthorizationDelegatingHandler>();

        // User context
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
    }

    public static void ConfigureConsumers(IRegistrationConfigurator registrationConfigurator)
    {
        // Register integration event consumers
        // For now, no consumers are needed as Identity doesn't consume events from other modules
        // This method is here for future use when cross-module events are needed
    }
}
