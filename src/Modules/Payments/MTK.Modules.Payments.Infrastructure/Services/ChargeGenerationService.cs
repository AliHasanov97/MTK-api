using Microsoft.Extensions.Logging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Charges.Services;
using MTK.Modules.Payments.Application.OwnerBalances.Services;
using MTK.Modules.Payments.Application.Payments.Services;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Services;

/// <summary>
/// Aylıq borcların generasiyasını koordinasiya edən əsas servis
/// </summary>
internal sealed class ChargeGenerationService(
    ApartmentChargeGenerator apartmentChargeGenerator,
    GarageChargeGenerator garageChargeGenerator,
    PaymentsDbContext dbContext,
    IChargeRepository chargeRepository,
    IPaymentAllocationService paymentAllocationService,
    IOwnerBalanceService ownerBalanceService,
    ILogger<ChargeGenerationService> logger) : IChargeGenerationService
{
    public async Task<ChargeGenerationResult> GenerateMonthlyChargesAsync(
        string period,
        bool regenerateCancelled = false,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting monthly charge generation for period {Period}", period);

        DateTimeOffset issuedOn = ChargePeriod.FirstDay(period);

        // Bu dövr üçün artıq borcu olan əmlakları yüklə
        HashSet<Guid> billedPropertyIds = await chargeRepository.GetPropertyIdsWithChargeForPeriodAsync(
            period, includeCancelled: !regenerateCancelled, cancellationToken);

        var createdCharges = new List<Charge>();
        var affectedOwnerIds = new HashSet<Guid>();

        // 1. Mənzil borclarını yarat
        var apartmentResult = await apartmentChargeGenerator.GenerateChargesAsync(
            period, issuedOn, billedPropertyIds, createdCharges, affectedOwnerIds, cancellationToken);

        // 2. Qaraj borclarını yarat
        var garageResult = await garageChargeGenerator.GenerateChargesAsync(
            period, issuedOn, billedPropertyIds, createdCharges, affectedOwnerIds, cancellationToken);

        int totalChargesCreated = apartmentResult.Created + garageResult.Created;

        if (totalChargesCreated == 0)
        {
            logger.LogInformation("Monthly charge generation completed for period {Period}. No new charges", period);
            return new ChargeGenerationResult(0, apartmentResult.SkippedNoRate, garageResult.SkippedNoRate);
        }

        // 3. Saxla və avans tətbiq et
        await SaveAndApplyAdvanceAsync(period, createdCharges, affectedOwnerIds, cancellationToken);

        logger.LogInformation("Monthly charge generation completed for period {Period}. Created {Count} charges",
            period, totalChargesCreated);

        return new ChargeGenerationResult(totalChargesCreated, apartmentResult.SkippedNoRate, garageResult.SkippedNoRate);
    }

    private async Task SaveAndApplyAdvanceAsync(
        string period,
        List<Charge> createdCharges,
        HashSet<Guid> affectedOwnerIds,
        CancellationToken cancellationToken)
    {
        await using var transaction = await chargeRepository.BeginTransactionAsync(cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        // Avansla bağla
        Result advanceResult = await paymentAllocationService.ApplyAdvanceToChargesAsync(
            createdCharges, cancellationToken);

        if (advanceResult.IsFailure)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(
                "Advance settlement failed for period {Period} ({Code}: {Message}); generation rolled back",
                period, advanceResult.Error.Code, advanceResult.Error.Message);

            throw new InvalidOperationException(
                $"Avansla bağlama uğursuz oldu, '{period}' dövrü üçün generasiya geri qaytarıldı ({advanceResult.Error.Code}: {advanceResult.Error.Message}).");
        }

        // Balansı yenilə
        await ownerBalanceService.RecalculateAsync(affectedOwnerIds, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }
}

