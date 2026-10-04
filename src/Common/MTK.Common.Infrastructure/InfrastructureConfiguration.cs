using Amazon.Runtime;
using Amazon.S3;
using MassTransit;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MTK.Common.Application.Auditing;
using MTK.Common.Application.Data;
using MTK.Common.Application.EventBus;
using MTK.Common.Application.Messaging;
using MTK.Common.Application.Storage;
using MTK.Common.Infrastructure.Auditing;
using MTK.Common.Infrastructure.Database;
using MTK.Common.Infrastructure.EventBus;
using MTK.Common.Infrastructure.Messaging;
using MTK.Common.Infrastructure.Options;
using MTK.Common.Infrastructure.Outbox;
using MTK.Common.Infrastructure.Storage;
using Npgsql;
using Quartz;
using QuestPDF.Infrastructure;

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
        // Document generation — set once for every module that uses QuestPDF.
        QuestPDF.Settings.License = LicenseType.Community;

        // RabbitMQ Configuration
        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMQ"));

        // File storage (Cloudflare R2) — shared across all 3 modules (Buildings,
        // Identity, Payments), each with its own FileAttachment entity pointing at
        // objects in the same bucket. R2 speaks the S3 API, so the regular AWS SDK
        // works unmodified against its endpoint.
        services.Configure<CloudflareR2Options>(configuration.GetSection("CloudflareR2"));
        services.AddSingleton<IAmazonS3>(sp =>
        {
            var r2Options = sp.GetRequiredService<IOptions<CloudflareR2Options>>().Value;
            return new AmazonS3Client(
                // A trailing newline pasted into appsettings.json's AccessKey/SecretKey
                // corrupts the SigV4 signature silently (R2 just rejects the request) —
                // trimming here means a config typo fails loudly instead, via R2's own
                // "invalid credentials" error, not a cryptic signing-method complaint.
                r2Options.AccessKey.Trim(),
                r2Options.SecretKey.Trim(),
                new AmazonS3Config
                {
                    ServiceURL = $"https://{r2Options.AccountId.Trim()}.r2.cloudflarestorage.com",
                    ForcePathStyle = true,
                    AuthenticationRegion = "auto",
                    // AWSSDK.S3 v4's default (WHEN_SUPPORTED) opportunistically adds
                    // flexible-checksum trailers to the signed request; R2 doesn't
                    // support that signing variant and rejects it with a generic
                    // "Please use AWS4-HMAC-SHA256" instead of a checksum-specific
                    // error. WHEN_REQUIRED keeps plain SigV4.
                    RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED,
                    ResponseChecksumValidation = ResponseChecksumValidation.WHEN_REQUIRED,
                });
        });
        services.AddScoped<IFileStorageService, CloudflareR2FileStorageService>();

        // Core Services
        services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.TryAddScoped<IEventBus, EventBus.EventBus>();

        // Interceptors
        services.AddScoped<InsertOutboxMessagesInterceptor>();

        // Audit actor override for background (outbox-driven) domain event handlers
        services.AddScoped<IAuditActorAccessor, AuditActorAccessor>();

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
