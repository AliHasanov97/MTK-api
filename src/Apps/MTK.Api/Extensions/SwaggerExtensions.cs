using Microsoft.OpenApi.Models;

namespace MTK.Api.Extensions;

public static class SwaggerExtensions
{
    private const string KeycloakSecurityScheme = "Keycloak";

    /// <summary>Configures Swagger with Keycloak OAuth2 (implicit flow).</summary>
    public static IServiceCollection AddApiSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "MTK API",
                Version = "v1",
                Description = "Mənzil Təsərrüfatı Kompleksi API"
            });

            // Keycloak OAuth2 security definition using Implicit Flow
            options.AddSecurityDefinition(KeycloakSecurityScheme, new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(configuration["Keycloak:AuthorizationUrl"]!),
                        Scopes = new Dictionary<string, string>
                        {
                            { "openid", "openid" },
                            { "profile", "profile" }
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
                            Id = KeycloakSecurityScheme
                        },
                        In = ParameterLocation.Header,
                        Name = "Bearer",
                        Scheme = "Bearer"
                    },
                    Array.Empty<string>()
                }
            });

            // Use fully qualified names to avoid schema ID collisions between modules
            options.CustomSchemaIds(type => type.FullName?.Replace("+", ".") ?? type.Name);
        });

        return services;
    }

    public static WebApplication UseApiSwagger(this WebApplication app, IConfiguration configuration)
    {
        if (!app.Environment.IsDevelopment() && !configuration.GetValue("Swagger:Enabled", true))
        {
            return app;
        }

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "MTK API V1");
            options.OAuthClientId(configuration["Keycloak:AuthClientId"]);
            options.OAuthScopes("openid", "profile");
        });

        return app;
    }
}
