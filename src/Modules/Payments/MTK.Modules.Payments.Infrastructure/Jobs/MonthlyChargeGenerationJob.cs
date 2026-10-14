using System.Runtime.ExceptionServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.Charges.Services;
using Quartz;

namespace MTK.Modules.Payments.Infrastructure.Jobs;

/// <summary>
/// Cari dövr ("yyyy-MM", Payments:MonthlyChargeGeneration:TimeZoneId qurşağına görə) üçün borc generasiyası.
/// İş gündəlik işləyir və idempotentdir: hələ borcu olmayan əmlak/xidmətlərə borc yaradır, ləğv edilmiş borcu təkrar yaratmır.
/// </summary>
[DisallowConcurrentExecution]
internal sealed class MonthlyChargeGenerationJob(
    IServiceScopeFactory serviceScopeFactory,
    IDateTimeProvider dateTimeProvider,
    IOptions<MonthlyChargeGenerationOptions> options,
    ILogger<MonthlyChargeGenerationJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        string currentPeriod = ChargePeriod.Current(dateTimeProvider.UtcNow, options.Value.ResolveTimeZone());

        logger.LogInformation("Payments - Beginning charge generation for period {Period}", currentPeriod);

        // Sakin və tədarükçü generasiyası bir-birini bloklamasın: ikisi də işləyir, xəta olsa sonda job uğursuz kimi görünür.
        // Hər biri öz scope-unda (öz DbContext-i ilə) işləyir: birincinin geri qaytarılmış/yazılmamış
        // entity-ləri izləyicidə qalıb ikincinin SaveChanges-inə düşməsin.
        Exception? failure = null;

        try
        {
            using IServiceScope scope = serviceScopeFactory.CreateScope();
            var chargeService = scope.ServiceProvider.GetRequiredService<IChargeGenerationService>();
            ChargeGenerationResult result = await chargeService.GenerateMonthlyChargesAsync(
                currentPeriod, cancellationToken: context.CancellationToken);

            logger.LogInformation(
                "Payments - Resident charge generation completed for {Period}. Created {Created}, apartments without rate {ApartmentsNoRate}, garages without rate {GaragesNoRate}",
                currentPeriod, result.Created, result.ApartmentsSkippedNoRate, result.GaragesSkippedNoRate);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Payments - Resident charge generation failed for period {Period}", currentPeriod);
            failure = ex;
        }

        try
        {
            using IServiceScope scope = serviceScopeFactory.CreateScope();
            var vendorChargeService = scope.ServiceProvider.GetRequiredService<IVendorChargeGenerationService>();
            int vendorChargesCreated = await vendorChargeService.GenerateScheduledChargesAsync(
                currentPeriod, cancellationToken: context.CancellationToken);

            logger.LogInformation(
                "Payments - Vendor charge generation completed for {Period}. Created {Count}",
                currentPeriod, vendorChargesCreated);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Payments - Vendor charge generation failed for period {Period}", currentPeriod);
            failure ??= ex;
        }

        try
        {
            using IServiceScope scope = serviceScopeFactory.CreateScope();
            var customerChargeService = scope.ServiceProvider.GetRequiredService<ICustomerChargeGenerationService>();
            int customerChargesCreated = await customerChargeService.GenerateScheduledChargesAsync(
                currentPeriod, cancellationToken: context.CancellationToken);

            logger.LogInformation(
                "Payments - Customer charge generation completed for {Period}. Created {Count}",
                currentPeriod, customerChargesCreated);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Payments - Customer charge generation failed for period {Period}", currentPeriod);
            failure ??= ex;
        }

        if (failure is not null)
        {
            ExceptionDispatchInfo.Capture(failure).Throw();
        }
    }
}
