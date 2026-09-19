using Microsoft.OpenApi.Models;
using MTK.Common.Application.Behaviors;
using MTK.Modules.Identity.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with Keycloak OAuth2
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MTK API",
        Version = "v1",
        Description = "Mənzil Təsərrüfatı Kompleksi API"
    });

    // Add Keycloak OAuth2 security definition
    options.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri(builder.Configuration["Keycloak:AuthorizationUrl"]!),
                TokenUrl = new Uri(builder.Configuration["Keycloak:TokenUrl"]!),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "OpenID" },
                    { "profile", "Profile" },
                    { "email", "Email" }
                }
            }
        }
    });

    // Require OAuth2 for all operations
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Keycloak"
                }
            },
            new[] { "openid", "profile" }
        }
    });
});

// ========== MODULAR MONOLITH STRUCTURE ==========

// Add Common services (MediatR, FluentValidation, AutoMapper)
builder.Services.AddMediatR(config =>
{
    // Register all handlers from all modules
    config.RegisterServicesFromAssemblyContaining<Program>();

    // Add ValidationBehavior globally
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

// Add Modules
builder.Services.AddIdentityModule(builder.Configuration);

// TODO: Register other modules here
// builder.Services.AddPropertiesModule(builder.Configuration);
// builder.Services.AddTenantsModule(builder.Configuration);
// builder.Services.AddPaymentsModule(builder.Configuration);
// builder.Services.AddServicesModule(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MTK API V1");
        options.OAuthClientId(builder.Configuration["Keycloak:AuthClientId"]);
        options.OAuthScopes("openid", "profile", "email");
        options.OAuthUsePkce();
        options.EnablePersistAuthorization();
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
