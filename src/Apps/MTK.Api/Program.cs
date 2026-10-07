using MTK.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ========== SERVICES ==========

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddFrontendCors(builder.Configuration);
builder.Services.AddApiSwagger(builder.Configuration);

// ========== MODULAR MONOLITH STRUCTURE ==========

builder.Services.AddCommonServices(builder.Configuration);
builder.Services.AddApplicationModules(builder.Configuration);

var app = builder.Build();

// ========== STARTUP CHECKS ==========

// Fail fast on any invalid AutoMapper configuration (e.g. a renamed/computed
// destination member wired with ForMember instead of ForCtorParam on a record
// with no parameterless constructor) at startup, instead of on the first request
// that happens to hit that particular mapping.
app.Services.GetRequiredService<AutoMapper.IConfigurationProvider>().AssertConfigurationIsValid();

// Automatically apply pending migrations on startup
await app.Services.ApplyMigrationsAsync();

// ========== HTTP REQUEST PIPELINE ==========

app.UseApiSwagger(builder.Configuration);

app.UseHttpsRedirection();

app.UseFrontendCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
