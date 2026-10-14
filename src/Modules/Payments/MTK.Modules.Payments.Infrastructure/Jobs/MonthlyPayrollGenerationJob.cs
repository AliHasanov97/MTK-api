using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.Payroll.Services;
using Quartz;

namespace MTK.Modules.Payments.Infrastructure.Jobs;

/// <summary>
/// Əvvəlki təqvim ayı (Payments:MonthlyPayrollGeneration:TimeZoneId qurşağına görə) üçün
/// əmək haqqı hesablamasını avtomatik yaradır — əmək haqqı əvvəlcədən deyil, ay bitdikdən
/// sonra hesablanır (MonthlyChargeGenerationJob-dan fərqli olaraq, o, CARİ ay üçün borc yaradır).
/// İş gündəlik işləyir və idempotentdir: əvvəlki ay üçün PayrollRun artıq varsa, sadəcə keçir.
/// </summary>
[DisallowConcurrentExecution]
internal sealed class MonthlyPayrollGenerationJob(
    IServiceScopeFactory serviceScopeFactory,
    IDateTimeProvider dateTimeProvider,
    IOptions<MonthlyPayrollGenerationOptions> options,
    ILogger<MonthlyPayrollGenerationJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var timeZone = options.Value.ResolveTimeZone();
        var localNow = TimeZoneInfo.ConvertTime(dateTimeProvider.UtcNow, timeZone);
        var previousMonth = new DateOnly(localNow.Year, localNow.Month, 1).AddMonths(-1);

        logger.LogInformation(
            "Payments - Beginning payroll generation for period {Year}-{Month:D2}",
            previousMonth.Year, previousMonth.Month);

        using IServiceScope scope = serviceScopeFactory.CreateScope();
        var payrollRunGenerationService = scope.ServiceProvider.GetRequiredService<IPayrollRunGenerationService>();

        var result = await payrollRunGenerationService.GenerateAsync(
            previousMonth.Year, previousMonth.Month, context.CancellationToken);

        if (result.IsSuccess)
        {
            logger.LogInformation(
                "Payments - Payroll generation completed for {Year}-{Month:D2}: {Count} payslips, net total {Net}",
                previousMonth.Year, previousMonth.Month, result.Value.Payslips.Count, result.Value.TotalNet);
            return;
        }

        // Artıq hesablanıb — avtomatik işin gündəlik təkrarında gözlənilən, normal haldır.
        if (result.Error.Code == "PayrollRun.AlreadyExists")
        {
            logger.LogInformation(
                "Payments - Payroll for {Year}-{Month:D2} already generated, skipping",
                previousMonth.Year, previousMonth.Month);
            return;
        }

        logger.LogError(
            "Payments - Payroll generation failed for {Year}-{Month:D2}: {Code} — {Message}",
            previousMonth.Year, previousMonth.Month, result.Error.Code, result.Error.Message);

        throw new InvalidOperationException(
            $"Payroll generation failed for {previousMonth.Year}-{previousMonth.Month:D2}: {result.Error.Code} — {result.Error.Message}");
    }
}
