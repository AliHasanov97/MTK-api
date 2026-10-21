using System.Security.Cryptography;
using System.Text;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MTK.Common.Application.EventBus;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.IntegrationEvents.DebtReminders;
using MTK.Modules.Payments.Infrastructure.Database;
using Quartz;

namespace MTK.Modules.Payments.Infrastructure.Jobs;

[DisallowConcurrentExecution]
internal sealed class DebtReminderJob(
    PaymentsDbContext dbContext,
    IEventBus eventBus,
    IOptions<DebtReminderOptions> options,
    ILogger<DebtReminderJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var localDate = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById(options.Value.TimeZoneId)).Date;
        var monthKey = localDate.ToString("yyyy-MM");

        var debtRows = await dbContext.Charges.AsNoTracking()
            .Where(c => c.OwnerId != null
                        && c.Period != null
                        && (c.Status == ChargeStatus.Unpaid || c.Status == ChargeStatus.PartiallyPaid)
                        && c.Amount > c.PaidAmount)
            .Select(c => new
            {
                OwnerId = c.OwnerId!.Value,
                Period = c.Period!,
                c.Amount,
                c.PaidAmount,
                Balance = c.Amount - c.PaidAmount,
                PropertyId = c.ApartmentId ?? c.GarageId,
                PropertyLabel = c.ApartmentId != null
                    ? $"M\u0259nzil \u2116 {c.Apartment!.ApartmentNumber} \u00b7 {c.Apartment.Building.Name}"
                    : c.GarageId != null
                        ? $"Qaraj \u2116 {c.Garage!.GarageNumber}"
                        : "\u00dcmumi hesab"
                        })
            .ToListAsync(context.CancellationToken);

        var qualifyingOwners = debtRows
            .GroupBy(row => row.OwnerId)
            .Select(group => new
            {
                OwnerId = group.Key,
                Rows = group.Where(row => IsEarlierMonthlyPeriod(row.Period, localDate)).ToArray()
            })
            .Select(owner => new
            {
                owner.OwnerId,
                Periods = owner.Rows
                    .GroupBy(row => row.Period)
                    .Where(periodGroup => periodGroup.Sum(row => row.Balance) > 0)
                    .Select(periodGroup => periodGroup.Key)
                    .OrderBy(period => period)
                    .ToArray(),
                Amount = owner.Rows.Sum(row => row.Balance),
                Rows = owner.Rows
            })
            .Where(owner => owner.Periods.Length >= 2 && owner.Amount > 0)
            .ToArray();

        if (qualifyingOwners.Length == 0)
        {
            logger.LogInformation("Payments - No residents currently qualify for debt reminders");
            return;
        }

        var ownerIds = qualifyingOwners.Select(owner => owner.OwnerId).ToArray();
        var owners = await dbContext.Owners.AsNoTracking()
            .Where(owner => ownerIds.Contains(owner.Id))
            .Select(owner => new { owner.Id, owner.FullName, owner.Email })
            .ToDictionaryAsync(owner => owner.Id, context.CancellationToken);
        var balances = await dbContext.OwnerBalances.AsNoTracking()
            .Where(balance => ownerIds.Contains(balance.OwnerId))
            .Select(balance => new { balance.OwnerId, balance.TotalDebt, balance.TotalPaid })
            .ToDictionaryAsync(balance => balance.OwnerId, context.CancellationToken);

        foreach (var qualifyingOwner in qualifyingOwners)
        {
            if (!owners.TryGetValue(qualifyingOwner.OwnerId, out var owner)
                || string.IsNullOrWhiteSpace(owner.Email))
            {
                logger.LogWarning("Payments - Debt reminder skipped for owner {OwnerId}: email is missing", qualifyingOwner.OwnerId);
                continue;
            }

            // Stable event ID means a repeated daily scan in the same month is deduplicated by the Communications inbox.
            var eventId = CreateMonthlyEventId(owner.Id, monthKey);
            var propertyDetails = qualifyingOwner.Rows
                .GroupBy(row => new { row.PropertyId, row.PropertyLabel })
                .Select(property =>
                {
                    var periods = property.GroupBy(row => row.Period)
                        .OrderBy(period => period.Key)
                        .Select(period => new DebtReminderPeriodDetail(
                            period.Key,
                            period.Sum(row => row.Amount),
                            period.Sum(row => row.PaidAmount),
                            period.Sum(row => row.Balance)))
                        .ToArray();
                    return new DebtReminderPropertyDetail(
                        property.Key.PropertyLabel,
                        periods,
                        periods.Sum(period => period.Amount),
                        periods.Sum(period => period.PaidAmount),
                        periods.Sum(period => period.OutstandingAmount));
                })
                .OrderByDescending(property => property.OutstandingAmount)
                .ToArray();
            balances.TryGetValue(owner.Id, out var ownerBalance);
            await eventBus.PublishAsync(new DebtReminderRequestedIntegrationEvent(
                eventId,
                DateTime.UtcNow,
                owner.Id,
                owner.FullName,
                owner.Email,
                qualifyingOwner.Periods,
                qualifyingOwner.Amount,
                ownerBalance?.TotalDebt ?? qualifyingOwner.Rows.Sum(row => row.Amount),
                ownerBalance?.TotalPaid ?? qualifyingOwner.Rows.Sum(row => row.PaidAmount),
                propertyDetails), context.CancellationToken);
        }

        logger.LogInformation("Payments - Published debt reminders for {Count} residents", qualifyingOwners.Length);
    }

    private static Guid CreateMonthlyEventId(Guid ownerId, string month)
    {
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes($"debt-reminder:{ownerId:N}:{month}"));
        return new Guid(hash.AsSpan(0, 16));
    }

    private static bool IsEarlierMonthlyPeriod(string period, DateTime currentDate) =>
        DateTime.TryParseExact(period, "yyyy-MM", CultureInfo.InvariantCulture,
            DateTimeStyles.None, out var chargeMonth)
        && chargeMonth < new DateTime(currentDate.Year, currentDate.Month, 1);
}
