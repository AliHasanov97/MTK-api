using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MTK.Common.Application.EventBus;
using MTK.Common.Infrastructure.EventBus;
using IUnitOfWork = MTK.Modules.Payments.Application.Abstractions.Data.IUnitOfWork;
using MTK.Common.Infrastructure.Inbox;
using MTK.Common.Infrastructure.Outbox;
using MTK.Modules.Payments.Application.Abstractions.Services.Export;
using MTK.Modules.Payments.Application.Payments.Commands.CreatePayment;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Database;
using MTK.Modules.Payments.Infrastructure.Export;
using MTK.Modules.Payments.Infrastructure.Inbox;
using MTK.Modules.Payments.Infrastructure.Repositories;
using MTK.Modules.Buildings.IntegrationEvents.Apartments;
using MTK.Modules.Buildings.IntegrationEvents.Garages;
using MTK.Modules.Buildings.IntegrationEvents.OwnershipHistories;
using MTK.Modules.Identity.IntegrationEvents.Users;
using Outbox = MTK.Modules.Payments.Infrastructure.Outbox;

namespace MTK.Modules.Payments.Infrastructure;

public static class PaymentsModule
{
    public static IServiceCollection AddPaymentsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Application layer
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreatePaymentCommand).Assembly);
        });

        services.AddValidatorsFromAssembly(
            typeof(CreatePaymentCommand).Assembly,
            includeInternalTypes: true);

        // Domain Event Handlers (for ProcessOutboxJob)
        services.AddDomainEventHandlers();

        // Integration Event Handlers (for ProcessInboxJob)
        services.AddIntegrationEventHandlers();

        // Database
        services.AddHttpContextAccessor();
        services.AddDbContext<PaymentsDbContext>((sp, options) =>
        {
            var outboxInterceptor = sp.GetRequiredService<InsertOutboxMessagesInterceptor>();

            options.UseNpgsql(
                    configuration.GetConnectionString("Database"),
                    npgsqlOptions => npgsqlOptions.MigrationsHistoryTable(
                        "__EFMigrationsHistory",
                        "payments"))
                .AddInterceptors(outboxInterceptor);
        });

        // Unit of Work
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<PaymentsDbContext>());

        // Repositories
        services.AddScoped<IRateRepository, RateRepository>();
        services.AddScoped<IChargeRepository, ChargeRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IPaymentAllocationRepository, PaymentAllocationRepository>();
        services.AddScoped<IOwnerBalanceRepository, OwnerBalanceRepository>();
        services.AddScoped<ICompanyBalanceRepository, CompanyBalanceRepository>();
        services.AddScoped<IPropertyOwnershipRepository, PropertyOwnershipRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IVendorRepository, VendorRepository>();
        services.AddScoped<IContractRepository, ContractRepository>();
        services.AddScoped<IVendorChargeRepository, VendorChargeRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Export services (PDF/Excel)
        services.AddScoped<IPaymentReceiptExportService, PaymentReceiptExportService>();
        services.AddScoped<IAnnualPaymentReportExcelExportService, AnnualPaymentReportExcelExportService>();
        services.AddScoped<IContractExportService, ContractExportService>();

        // Services
        services.AddScoped<MTK.Modules.Payments.Application.Payments.Services.IPaymentAllocationService, MTK.Modules.Payments.Application.Payments.Services.PaymentAllocationService>();
        services.AddScoped<MTK.Modules.Payments.Application.OwnerBalances.Services.IOwnerBalanceService, MTK.Modules.Payments.Application.OwnerBalances.Services.OwnerBalanceService>();
        services.AddScoped<MTK.Modules.Payments.Application.CompanyBalances.Services.ICompanyBalanceService, MTK.Modules.Payments.Application.CompanyBalances.Services.CompanyBalanceService>();
        services.AddScoped<MTK.Modules.Payments.Application.Charges.Services.IChargeGenerationService, MTK.Modules.Payments.Infrastructure.Services.ChargeGenerationService>();
        services.AddScoped<MTK.Modules.Payments.Application.Charges.Services.IVendorChargeGenerationService, MTK.Modules.Payments.Infrastructure.Services.VendorChargeGenerationService>();

        // Outbox & Inbox Configuration
        services.Configure<OutboxOptions>(configuration.GetSection("Payments:Outbox"));
        services.Configure<InboxOptions>(configuration.GetSection("Payments:Inbox"));

        // Monthly Charge Generation Configuration
        services.Configure<Jobs.MonthlyChargeGenerationOptions>(
            configuration.GetSection("Payments:MonthlyChargeGeneration"));

        // Quartz Job Configurators
        services.ConfigureOptions<Outbox.ConfigureProcessOutboxJob>();
        services.ConfigureOptions<Inbox.ConfigureProcessInboxJob>();
        services.ConfigureOptions<Jobs.ConfigureMonthlyChargeGenerationJob>();

        return services;
    }

    private static void AddDomainEventHandlers(this IServiceCollection services)
    {
        // Find all domain event handlers in Application assembly
        Type[] domainEventHandlers = typeof(CreatePaymentCommand).Assembly
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
        // Register integration event consumers from Buildings module
        registrationConfigurator.AddConsumer<IntegrationEventConsumer<ApartmentCreatedIntegrationEvent>>();
        registrationConfigurator.AddConsumer<IntegrationEventConsumer<ApartmentOwnerChangedIntegrationEvent>>();
        registrationConfigurator.AddConsumer<IntegrationEventConsumer<ApartmentUpdatedIntegrationEvent>>();
        registrationConfigurator.AddConsumer<IntegrationEventConsumer<GarageCreatedIntegrationEvent>>();
        registrationConfigurator.AddConsumer<IntegrationEventConsumer<GarageOwnerChangedIntegrationEvent>>();
        registrationConfigurator.AddConsumer<IntegrationEventConsumer<GarageOwnerRemovedIntegrationEvent>>();
        registrationConfigurator.AddConsumer<IntegrationEventConsumer<OwnershipTransferredIntegrationEvent>>();

        // Register integration event consumers from Identity module — used to keep
        // this module's own User snapshot (AuditLog.UserId -> readable name) in sync.
        registrationConfigurator.AddConsumer<IntegrationEventConsumer<UserCreatedIntegrationEvent>>();
        registrationConfigurator.AddConsumer<IntegrationEventConsumer<UserUpdatedIntegrationEvent>>();
        registrationConfigurator.AddConsumer<IntegrationEventConsumer<UserDeletedIntegrationEvent>>();
    }
}
