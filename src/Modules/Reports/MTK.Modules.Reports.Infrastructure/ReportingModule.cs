using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MTK.Modules.Reports.Application.Abstractions;
using MTK.Modules.Reports.Application.MonthlyReports;
using MTK.Modules.Reports.Infrastructure.Export;

namespace MTK.Modules.Reports.Infrastructure;

public static class ReportingModule
{
    public static IServiceCollection AddReportingModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(GetMonthlyReportQuery).Assembly);
        });

        services.AddScoped<IMonthlyReportPdfExportService, MonthlyReportPdfExportService>();

        return services;
    }
}
