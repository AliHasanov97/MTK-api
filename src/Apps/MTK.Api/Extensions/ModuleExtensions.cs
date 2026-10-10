using MTK.Common.Application.Behaviors;
using MTK.Common.Infrastructure;
using MTK.Modules.Buildings.Application;
using MTK.Modules.Buildings.Infrastructure;
using MTK.Modules.Hr.Infrastructure;
using MTK.Modules.Identity.Application;
using MTK.Modules.Identity.Infrastructure;
using MTK.Modules.Payments.Application;
using MTK.Modules.Payments.Infrastructure;
using MTK.Modules.Warehouse.Application;
using MTK.Modules.Warehouse.Infrastructure;

namespace MTK.Api.Extensions;

public static class ModuleExtensions
{
    /// <summary>
    /// Registers the shared infrastructure (Quartz, MassTransit, RabbitMQ, Outbox/Inbox)
    /// and the cross-cutting services used by every module (MediatR, AutoMapper).
    /// </summary>
    public static IServiceCollection AddCommonServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddInfrastructure(
            serviceName: "MTK API",
            moduleConfigureConsumers: [
                IdentityModule.ConfigureConsumers,
                BuildingsModule.ConfigureConsumers,
                PaymentsModule.ConfigureConsumers,
                WarehouseModule.ConfigureConsumers,
                HrModule.ConfigureConsumers
            ],
            databaseConnectionString: configuration.GetConnectionString("Database")!,
            configuration: configuration);

        services.AddMediatR(config =>
        {
            // Register all handlers from all modules
            config.RegisterServicesFromAssemblyContaining<Program>();

            // Add ValidationBehavior globally
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // AutoMapper must be registered exactly once here, scanning every module's Profile
        // classes together — calling AddAutoMapper per-module would build a separate
        // MapperConfiguration each time and the last call would silently win over the rest.
        services.AddAutoMapper(
            cfg => { },
            typeof(PaymentsMappingProfile).Assembly,
            typeof(BuildingsMappingProfile).Assembly,
            typeof(IdentityMappingProfile).Assembly,
            typeof(WarehouseMappingProfile).Assembly,
            MTK.Modules.Hr.Application.AssemblyReference.Assembly);

        return services;
    }

    /// <summary>Registers every bounded-context module of the monolith.</summary>
    public static IServiceCollection AddApplicationModules(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddIdentityModule(configuration);
        services.AddBuildingsModule(configuration);
        services.AddPaymentsModule(configuration);
        services.AddWarehouseModule(configuration);
        services.AddHrModule(configuration);

        // TODO: Register other modules here
        // services.AddBillingModule(configuration);
        // services.AddFinanceModule(configuration);
        // services.AddExpensesModule(configuration);
        // services.AddEmployeesModule(configuration);
        // services.AddMaintenanceModule(configuration);
        // services.AddVotingModule(configuration);

        return services;
    }
}
