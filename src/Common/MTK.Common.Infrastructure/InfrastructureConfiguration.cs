using MassTransit;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MTK.Common.Application.Data;
using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Common.Infrastructure.Database;
using MTK.Common.Infrastructure.EventBus;
using MTK.Common.Infrastructure.Messaging;
using MTK.Common.Infrastructure.Options;
using MTK.Common.Infrastructure.Outbox;
using Npgsql;
using Quartz;

namespace MTK.Common.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string serviceName,
        Action<IRegistrationConfigurator>[] moduleConfigureConsumers,
        string databaseConnectionString,
        IConfiguration configuration)
    {
        // RabbitMQ Configuration
        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMQ"));

        // Core Services
        services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.TryAddScoped<IEventBus, EventBus.EventBus>();

        // Interceptors
        services.AddScoped<InsertOutboxMessagesInterceptor>();

        // PostgreSQL Connection with Pooling
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(databaseConnectionString);
        dataSourceBuilder.EnableDynamicJson();
        dataSourceBuilder.ConnectionStringBuilder.MaxPoolSize = 50;
        dataSourceBuilder.ConnectionStringBuilder.MinPoolSize = 5;
        dataSourceBuilder.ConnectionStringBuilder.ConnectionIdleLifetime = 300;
        dataSourceBuilder.ConnectionStringBuilder.ConnectionPruningInterval = 10;
        dataSourceBuilder.ConnectionStringBuilder.CommandTimeout = 30;
        dataSourceBuilder.ConnectionStringBuilder.Timeout = 15;
        NpgsqlDataSource npgsqlDataSource = dataSourceBuilder.Build();
        services.TryAddSingleton(npgsqlDataSource);

        services.TryAddScoped<IDbConnectionFactory, DbConnectionFactory>();
        services.TryAddScoped<IDbTransactionAccessor, DbTransactionAccessor>();

        // Quartz Scheduler
        services.AddQuartz(configurator =>
        {
            var scheduler = Guid.NewGuid();
            configurator.SchedulerId = $"mtk-scheduler-{scheduler}";
            configurator.SchedulerName = $"mtk-scheduler-name-{scheduler}";
        });
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        // MassTransit Configuration
        services.AddMassTransit(configure =>
        {
            // Register module consumers
            foreach (Action<IRegistrationConfigurator> configureConsumers in moduleConfigureConsumers)
            {
                configureConsumers(configure);
            }

            configure.SetKebabCaseEndpointNameFormatter();
            configure.AddDelayedMessageScheduler();

            // RabbitMQ Transport
            configure.UsingRabbitMq((context, cfg) =>
            {
                var rabbitOptions = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

                cfg.Host(rabbitOptions.Host, rabbitOptions.Port, rabbitOptions.VirtualHost, h =>
                {
                    h.Username(rabbitOptions.Username);
                    h.Password(rabbitOptions.Password);
                });

                cfg.UseDelayedMessageScheduler();

                // Retry Policy: 3 retries with exponential backoff
                cfg.UseMessageRetry(r => r.Intervals(
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10)));

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
