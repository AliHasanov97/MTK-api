using System.Globalization;
using Microsoft.Extensions.Logging;
using MTK.Modules.Payments.Application.Charges.Services;
using MTK.Modules.Payments.Application.OwnerBalances.Services;
using MTK.Modules.Payments.Application.Payments.Services;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.PropertyOwnerships;
using MTK.Modules.Payments.Domain.Rates;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Services;

internal sealed class ChargeGenerationService(
    PaymentsDbContext dbContext,
    IRateRepository rateRepository,
    IChargeRepository chargeRepository,
    IPropertyOwnershipRepository propertyOwnershipRepository,
    IPaymentAllocationService paymentAllocationService,
    IOwnerBalanceService ownerBalanceService,
    ILogger<ChargeGenerationService> logger) : IChargeGenerationService
{
    public async Task<int> GenerateMonthlyChargesAsync(string period, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting monthly charge generation for period {Period}", period);

        // Get active rates
        Rate? apartmentRate = await rateRepository.GetCurrentRateAsync(RateType.PerSquareMeter, cancellationToken);

        int chargesCreated = 0;

        // Avansla bağlama ikinci keçiddə, borclar yazıldıqdan sonra aparılır. Borcların
        // özü (referansları) saxlanılır — Id-nin EF tərəfindən nə vaxt verilməsindən
        // asılı olmasın deyə.
        var createdCharges = new List<Charge>();
        var affectedOwnerIds = new HashSet<Guid>();

        DateTimeOffset issuedOn = IssuedOnFromPeriod(period);

        // Get apartments with owners from PropertyOwnership read model
        List<PropertyOwnership> apartments = await propertyOwnershipRepository
            .GetByPropertyTypeAsync(PropertyType.Apartment, cancellationToken);

        // Generate charges for apartments
        if (apartmentRate is not null)
        {
            foreach (PropertyOwnership ownership in apartments)
            {
                Guid apartmentId = ownership.ApartmentId!.Value;
                decimal area = ownership.Apartment!.AreaSquareMeters;

                // Check if charge already exists for this period
                bool chargeExists = await chargeRepository.ChargeExistsForPeriodAsync(
                    apartmentId,
                    period,
                    cancellationToken);

                if (chargeExists)
                {
                    logger.LogDebug("Charge already exists for apartment {ApartmentId} in period {Period}",
                        apartmentId, period);
                    continue;
                }

                // Calculate charge amount: rate per m² × area
                decimal chargeAmount = apartmentRate.Amount * area;

                // Create charge with snapshot data
                Charge charge = Charge.CreateForApartment(
                    ownership.OwnerId,
                    apartmentId,
                    period,
                    issuedOn,
                    chargeAmount,
                    apartmentRate.Amount,     // Snapshot: rate at time of creation
                    RateType.PerSquareMeter,  // Snapshot: rate type
                    area);                    // Snapshot: area at time of creation

                chargeRepository.Add(charge);
                createdCharges.Add(charge);
                affectedOwnerIds.Add(ownership.OwnerId);

                chargesCreated++;

                logger.LogDebug("Created charge for apartment {ApartmentId}, owner {OwnerId}, amount {Amount}",
                    apartmentId, ownership.OwnerId, chargeAmount);
            }
        }

        // Get garages with owners from PropertyOwnership read model
        List<PropertyOwnership> garages = await propertyOwnershipRepository
            .GetByPropertyTypeAsync(PropertyType.Garage, cancellationToken);

        // Rate resolution is per garage type (falls back to the type-less default rate),
        // cached here so we don't re-query the same type for every garage. "null" (no
        // specific garage type) is tracked as its own cache key via HasRateFor.
        var garageRateByType = new Dictionary<GarageType, Rate?>();
        Rate? defaultGarageRate = null;
        bool defaultGarageRateResolved = false;

        foreach (PropertyOwnership ownership in garages)
        {
            Guid garageId = ownership.GarageId!.Value;
            GarageType? garageType = ownership.Garage!.GarageType;
            Rate? garageRate;

            if (garageType is { } specificType)
            {
                if (!garageRateByType.TryGetValue(specificType, out garageRate))
                {
                    garageRate = await rateRepository.GetCurrentGarageRateAsync(specificType, cancellationToken);
                    garageRateByType[specificType] = garageRate;
                }
            }
            else
            {
                if (!defaultGarageRateResolved)
                {
                    defaultGarageRate = await rateRepository.GetCurrentGarageRateAsync(null, cancellationToken);
                    defaultGarageRateResolved = true;
                }

                garageRate = defaultGarageRate;
            }

            if (garageRate is null)
            {
                logger.LogWarning(
                    "No active FixedGarage rate found for garage type {GarageType} (garage {GarageId}); skipping",
                    garageType, garageId);
                continue;
            }

            // Check if charge already exists for this period
            bool chargeExists = await chargeRepository.ChargeExistsForPeriodAsync(
                garageId,
                period,
                cancellationToken);

            if (chargeExists)
            {
                logger.LogDebug("Charge already exists for garage {GarageId} in period {Period}",
                    garageId, period);
                continue;
            }

            decimal chargeAmount = garageRate.Amount;

            // Create charge with snapshot data
            Charge charge = Charge.CreateForGarage(
                ownership.OwnerId,
                garageId,
                period,
                issuedOn,
                chargeAmount,
                garageRate.Amount,     // Snapshot: rate at time of creation
                RateType.FixedGarage); // Snapshot: rate type (no area for garages)

            chargeRepository.Add(charge);
            createdCharges.Add(charge);
            affectedOwnerIds.Add(ownership.OwnerId);

            chargesCreated++;

            logger.LogDebug("Created charge for garage {GarageId} (type {GarageType}), owner {OwnerId}, amount {Amount}",
                garageId, garageType, ownership.OwnerId, chargeAmount);
        }

        if (chargesCreated == 0)
        {
            logger.LogInformation("Monthly charge generation completed for period {Period}. No new charges", period);
            return 0;
        }

        // Borcların yazılması, avansdan bağlanması və balansın yenilənməsi bir
        // tranzaksiyadır: yarımçıq vəziyyətdə qala bilməz.
        await using var transaction = await chargeRepository.BeginTransactionAsync(cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        // İkinci keçid: yeni borcları sahibin qalan avansından bağla. Bütün borclar bir
        // çağırışda gedir — sahib üzrə avans bir dəfə oxunur (əvvəllər hər borc üçün
        // ayrı oxuma + ayrı SaveChanges olurdu).
        var advanceResult = await paymentAllocationService.ApplyAdvanceToChargesAsync(createdCharges, cancellationToken);

        if (advanceResult.IsFailure)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(
                "Advance settlement failed for period {Period} ({Code}: {Message}); generation rolled back",
                period, advanceResult.Error.Code, advanceResult.Error.Message);
            return 0;
        }

        // Balans aqreqatdan mütləq yenidən hesablanır (artırmalı yanaşma yoxdur).
        await ownerBalanceService.RecalculateAsync(affectedOwnerIds, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        logger.LogInformation("Monthly charge generation completed for period {Period}. Created {Count} charges",
            period, chargesCreated);

        return chargesCreated;
    }

    /// <summary>
    /// "yyyy-MM" dövrünü borcun yaşına çevirir — dövrün ilk günü. Format tanınmasa
    /// (gözlənilməyən dəyər) borc indi yaranmış sayılır.
    /// </summary>
    private static DateTimeOffset IssuedOnFromPeriod(string period)
    {
        return DateTimeOffset.TryParseExact(
            period + "-01",
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
            out DateTimeOffset parsed)
            ? parsed
            : DateTimeOffset.UtcNow;
    }
}
