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

        var chargeService = scope.ServiceProvider.GetRequiredService<IChargeGenerationService>();
        string currentPeriod = dateTimeProvider.UtcNow.ToString("yyyy-MM");
        int chargesCreated = await chargeService.GenerateMonthlyChargesAsync(currentPeriod, context.CancellationToken);

        logger.LogInformation("Payments - Monthly charge generation completed. Created {ChargesCount} charges", chargesCreated);
    }
}
