using Microsoft.Extensions.Logging;
using MTK.Modules.Payments.Application.Charges.Services;
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
    ILogger<ChargeGenerationService> logger) : IChargeGenerationService
{
    public async Task<int> GenerateMonthlyChargesAsync(string period, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting monthly charge generation for period {Period}", period);

        // Get active rates
        Rate? apartmentRate = await rateRepository.GetCurrentRateAsync(RateType.PerSquareMeter, cancellationToken);
        Rate? garageRate = await rateRepository.GetCurrentRateAsync(RateType.FixedGarage, cancellationToken);

        if (apartmentRate is null && garageRate is null)
        {
            logger.LogWarning("No active rates found for period {Period}", period);
            return 0;
        }

        int chargesCreated = 0;

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

                // Create charge
                Charge charge = Charge.Create(
                    apartment.OwnerId,
                    PropertyType.Apartment,
                    apartment.PropertyId,
                    period,
                    chargeAmount);

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

                chargesCreated++;

                logger.LogDebug("Created charge for apartment {PropertyId}, owner {OwnerId}, amount {Amount}",
                    apartment.PropertyId, apartment.OwnerId, chargeAmount);
            }
        }

        // Get garages with owners from PropertyOwnership read model
        List<PropertyOwnership> garages = await propertyOwnershipRepository
            .GetByPropertyTypeAsync(PropertyType.Garage, cancellationToken);

        // Generate charges for garages
        if (garageRate is not null)
        {
            foreach (PropertyOwnership garage in garages)
            {
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

                // For garages, use fixed rate
                decimal chargeAmount = garageRate.Amount;

                // Create charge
                Charge charge = Charge.Create(
                    garage.OwnerId,
                    PropertyType.Garage,
                    garage.PropertyId,
                    period,
                    chargeAmount);

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

                chargesCreated++;

                logger.LogDebug("Created charge for garage {PropertyId}, owner {OwnerId}, amount {Amount}",
                    garage.PropertyId, garage.OwnerId, chargeAmount);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Monthly charge generation completed for period {Period}. Created {Count} charges",
            period, chargesCreated);

        return chargesCreated;
    }
}
