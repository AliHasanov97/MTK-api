using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MTK.Common.Application.Behaviors;
using MTK.Common.Infrastructure;
using MTK.Modules.Buildings.Application;
using MTK.Modules.Buildings.Infrastructure;
using MTK.Modules.Buildings.Infrastructure.Database;
using MTK.Modules.Identity.Application;
using MTK.Modules.Identity.Infrastructure;
using MTK.Modules.Identity.Infrastructure.Database;
using MTK.Modules.Payments.Application;
using MTK.Modules.Payments.Infrastructure;
using MTK.Modules.Payments.Infrastructure.Database;
using MTK.Modules.Warehouse.Application;
using MTK.Modules.Warehouse.Infrastructure;
using MTK.Modules.Warehouse.Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Allow the Next.js frontend (a different origin) to call this API from the browser
const string FrontendCorsPolicy = "FrontendCorsPolicy";
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            // Not CORS-safelisted by default — without this, the browser's fetch()
            // can read the file body of an export response but not its filename.
            .WithExposedHeaders("Content-Disposition");
    });
});

// Configure Swagger with Keycloak OAuth2
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MTK API",
        Version = "v1",
        Description = "Mənzil Təsərrüfatı Kompleksi API"
    });

    // Add Keycloak OAuth2 security definition using Implicit Flow
    options.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            Implicit = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri(builder.Configuration["Keycloak:AuthorizationUrl"]!),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "openid" },
                    { "profile", "profile" }
                }
            }
        }
    });

    // Require OAuth2 for all operations
    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Keycloak"
                },
                In = ParameterLocation.Header,
                Name = "Bearer",
                Scheme = "Bearer"
            },
            Array.Empty<string>()
        }
    };
    options.AddSecurityRequirement(securityRequirement);

    // Use fully qualified names to avoid schema ID collisions between modules
    options.CustomSchemaIds(type => type.FullName?.Replace("+", ".") ?? type.Name);
});

// ========== MODULAR MONOLITH STRUCTURE ==========

// Add Common Infrastructure (Quartz, MassTransit, RabbitMQ, Outbox/Inbox)
builder.Services.AddInfrastructure(
    serviceName: "MTK API",
    moduleConfigureConsumers: [
        IdentityModule.ConfigureConsumers,
        BuildingsModule.ConfigureConsumers,
        PaymentsModule.ConfigureConsumers
    ],
    databaseConnectionString: builder.Configuration.GetConnectionString("Database")!,
    configuration: builder.Configuration);

// Add Common services (MediatR, FluentValidation, AutoMapper)
builder.Services.AddMediatR(config =>
{
    // Register all handlers from all modules
    config.RegisterServicesFromAssemblyContaining<Program>();

    // Add ValidationBehavior globally
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

// AutoMapper must be registered exactly once here, scanning every module's Profile
// classes together — calling AddAutoMapper per-module would build a separate
// MapperConfiguration each time and the last call would silently win over the rest.
builder.Services.AddAutoMapper(
    cfg => { },
    typeof(PaymentsMappingProfile).Assembly,
    typeof(BuildingsMappingProfile).Assembly,
    typeof(IdentityMappingProfile).Assembly,
    typeof(WarehouseMappingProfile).Assembly);

// Add Modules
builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddBuildingsModule(builder.Configuration);
builder.Services.AddPaymentsModule(builder.Configuration);
builder.Services.AddWarehouseModule(builder.Configuration);

// TODO: Register other modules here
// builder.Services.AddBillingModule(builder.Configuration);
// builder.Services.AddFinanceModule(builder.Configuration);
// builder.Services.AddExpensesModule(builder.Configuration);
// builder.Services.AddEmployeesModule(builder.Configuration);
// builder.Services.AddMaintenanceModule(builder.Configuration);
// builder.Services.AddVotingModule(builder.Configuration);

var app = builder.Build();

// Fail fast on any invalid AutoMapper configuration (e.g. a renamed/computed
// destination member wired with ForMember instead of ForCtorParam on a record
// with no parameterless constructor) at startup, instead of on the first request
// that happens to hit that particular mapping.
app.Services.GetRequiredService<AutoMapper.IConfigurationProvider>().AssertConfigurationIsValid();

// ========== AUTO MIGRATION ==========
// Automatically apply pending migrations on startup
await ApplyMigrationsAsync(app.Services);

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment() || app.Configuration.GetValue("Swagger:Enabled", true)){
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MTK API V1");
        options.OAuthClientId(builder.Configuration["Keycloak:AuthClientId"]);
        options.OAuthScopes("openid", "profile");
    });
}

app.UseHttpsRedirection();

app.UseCors(FrontendCorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// ========== HELPER METHODS ==========

static async Task ApplyMigrationsAsync(IServiceProvider serviceProvider)
{
    await using var scope = serviceProvider.CreateAsyncScope();

    // Apply Identity module migrations
    var identityDbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    await identityDbContext.Database.MigrateAsync();

    // Apply Buildings module migrations
    var buildingsDbContext = scope.ServiceProvider.GetRequiredService<BuildingsDbContext>();
    await buildingsDbContext.Database.MigrateAsync();

    // Apply Payments module migrations
    var paymentsDbContext = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
    await paymentsDbContext.Database.MigrateAsync();

    // Apply Warehouse module migrations
    var warehouseDbContext = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();
    await warehouseDbContext.Database.MigrateAsync();

    // TODO: Apply other module migrations here
}
