namespace MTK.Api.Extensions;

public static class CorsExtensions
{
    /// <summary>Name of the CORS policy used by the Next.js frontend.</summary>
    public const string FrontendCorsPolicy = "FrontendCorsPolicy";

    /// <summary>
    /// Allows the Next.js frontend (a different origin) to call this API from the browser.
    /// </summary>
    public static IServiceCollection AddFrontendCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

        services.AddCors(options =>
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

        return services;
    }

    public static WebApplication UseFrontendCors(this WebApplication app)
    {
        app.UseCors(FrontendCorsPolicy);
        return app;
    }
}
