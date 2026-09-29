using Microsoft.Extensions.Logging;
using MTK.Modules.Payments.Application.Charges.Services;
using MTK.Modules.Payments.Application.Payments.Services;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.OwnerBalances;
using MTK.Modules.Payments.Domain.PropertyOwnerships;
using MTK.Modules.Payments.Domain.Rates;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Services;

internal sealed class ChargeGenerationService(
    PaymentsDbContext dbContext,
    IRateRepository rateRepository,
    IChargeRepository chargeRepository,
    IOwnerBalanceRepository ownerBalanceRepository,
    IPropertyOwnershipRepository propertyOwnershipRepository,
    IPaymentAllocationService paymentAllocationService,
    ILogger<ChargeGenerationService> logger) : IChargeGenerationService
{
    public async Task<int> GenerateMonthlyChargesAsync(string period, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting monthly charge generation for period {Period}", period);

        // Get active rates
        Rate? apartmentRate = await rateRepository.GetCurrentRateAsync(RateType.PerSquareMeter, cancellationToken);

        int chargesCreated = 0;
        // Settled against owner advance in a second pass, after the SaveChangesAsync
        // below — keeping the Charge references (not their Ids) so this works
        // regardless of exactly when EF assigns the key.
        var createdCharges = new List<(Charge Charge, Guid OwnerId)>();

        // Get apartments with owners from PropertyOwnership read model
        List<PropertyOwnership> apartments = await propertyOwnershipRepository
            .GetByPropertyTypeAsync(PropertyType.Apartment, cancellationToken);

        // Generate charges for apartments
        if (apartmentRate is not null)
        {
            foreach (PropertyOwnership apartment in apartments)
            {
                // Check if charge already exists for this period
                bool chargeExists = await chargeRepository.ChargeExistsForPeriodAsync(
                    apartment.OwnerId,
                    apartment.PropertyId,
                    period,
                    cancellationToken);

                if (chargeExists)
                {
                    logger.LogDebug("Charge already exists for apartment {PropertyId} in period {Period}",
                        apartment.PropertyId, period);
                    continue;
                }

                // Calculate charge amount: rate per m² × area
                decimal chargeAmount = apartmentRate.Amount * apartment.AreaSquareMeters;

                // Create charge with snapshot data
                Charge charge = Charge.Create(
                    apartment.OwnerId,
                    PropertyType.Apartment,
                    apartment.PropertyId,
                    period,
                    chargeAmount,
                    apartmentRate.Amount,           // Snapshot: rate at time of creation
                    RateType.PerSquareMeter,        // Snapshot: rate type
                    apartment.AreaSquareMeters);    // Snapshot: area at time of creation

                chargeRepository.Add(charge);

                // Update owner balance
                OwnerBalance? ownerBalance = await ownerBalanceRepository.GetByOwnerIdAsync(
                    apartment.OwnerId,
                    cancellationToken);

                if (ownerBalance is null)
                {
                    ownerBalance = OwnerBalance.Create(apartment.OwnerId);
                    ownerBalanceRepository.Add(ownerBalance);
                }

                ownerBalance.AddCharge(chargeAmount);
                createdCharges.Add((charge, apartment.OwnerId));

                chargesCreated++;

                logger.LogDebug("Created charge for apartment {PropertyId}, owner {OwnerId}, amount {Amount}",
                    apartment.PropertyId, apartment.OwnerId, chargeAmount);
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

        foreach (PropertyOwnership garage in garages)
        {
            Rate? garageRate;

            if (garage.GarageType is { } specificType)
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
                    "No active FixedGarage rate found for garage type {GarageType} (garage {PropertyId}); skipping",
                    garage.GarageType, garage.PropertyId);
                continue;
            }

            // Check if charge already exists for this period
            bool chargeExists = await chargeRepository.ChargeExistsForPeriodAsync(
                garage.OwnerId,
                garage.PropertyId,
                period,
                cancellationToken);

            if (chargeExists)
            {
                logger.LogDebug("Charge already exists for garage {PropertyId} in period {Period}",
                    garage.PropertyId, period);
                continue;
            }

            decimal chargeAmount = garageRate.Amount;

            // Create charge with snapshot data
            Charge charge = Charge.Create(
                garage.OwnerId,
                PropertyType.Garage,
                garage.PropertyId,
                period,
                chargeAmount,
                garageRate.Amount,              // Snapshot: rate at time of creation
                RateType.FixedGarage,           // Snapshot: rate type
                null);                          // Snapshot: no area for garages

            chargeRepository.Add(charge);

            // Update owner balance
            OwnerBalance? ownerBalance = await ownerBalanceRepository.GetByOwnerIdAsync(
                garage.OwnerId,
                cancellationToken);

            if (ownerBalance is null)
            {
                ownerBalance = OwnerBalance.Create(garage.OwnerId);
                ownerBalanceRepository.Add(ownerBalance);
            }

            ownerBalance.AddCharge(chargeAmount);
            createdCharges.Add((charge, garage.OwnerId));

            chargesCreated++;

            logger.LogDebug("Created charge for garage {PropertyId} (type {GarageType}), owner {OwnerId}, amount {Amount}",
                garage.PropertyId, garage.GarageType, garage.OwnerId, chargeAmount);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        // Second pass: spend down any owner-level advance on these brand-new charges,
        // oldest owner-payment first. Runs after the save above so every charge has its
        // real, persisted Id.
        foreach (var (charge, ownerId) in createdCharges)
        {
            await paymentAllocationService.SettleChargeFromAdvanceAsync(charge.Id, ownerId, cancellationToken);
        }

        logger.LogInformation("Monthly charge generation completed for period {Period}. Created {Count} charges",
            period, chargesCreated);

        return chargesCreated;
    }
}
