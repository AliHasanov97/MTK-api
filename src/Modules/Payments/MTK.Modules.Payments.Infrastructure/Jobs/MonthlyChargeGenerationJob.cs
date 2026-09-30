using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.Charges.Services;
using Quartz;

namespace MTK.Modules.Payments.Infrastructure.Jobs;

[DisallowConcurrentExecution]
internal sealed class MonthlyChargeGenerationJob(
    IServiceScopeFactory serviceScopeFactory,
    IDateTimeProvider dateTimeProvider,
    ILogger<MonthlyChargeGenerationJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("Payments - Beginning monthly charge generation for period {Period}",
            dateTimeProvider.UtcNow.ToString("yyyy-MM"));

        using IServiceScope scope = serviceScopeFactory.CreateScope();

        string currentPeriod = dateTimeProvider.UtcNow.ToString("yyyy-MM");

        var chargeService = scope.ServiceProvider.GetRequiredService<IChargeGenerationService>();
        int chargesCreated = await chargeService.GenerateMonthlyChargesAsync(currentPeriod, context.CancellationToken);

        logger.LogInformation("Payments - Monthly charge generation completed. Created {ChargesCount} charges", chargesCreated);

        // Tədarükçü müqavilələri üzrə cədvəl borcları — eyni dövr üçün. Xəta
        // sakin borclarını yaradıb-törətməsin deyə ayrıca blokda saxlanılır.
        try
        {
            var vendorChargeService = scope.ServiceProvider.GetRequiredService<IVendorChargeGenerationService>();
            int vendorChargesCreated = await vendorChargeService.GenerateScheduledChargesAsync(
                currentPeriod, context.CancellationToken);

            logger.LogInformation(
                "Payments - Vendor charge generation completed. Created {ChargesCount} charges",
                vendorChargesCreated);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Payments - Vendor charge generation failed for period {Period}", currentPeriod);
        }
    }
}
