using Microsoft.Extensions.Logging;
using MTK.Modules.Payments.Application.PropertyOwnerships;
using MTK.Modules.Payments.Application.PropertyOwnerships.Services;
using MTK.Modules.Payments.Application.Rates.Queries.GetCurrentRates;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.PropertyOwnerships;
using MTK.Modules.Payments.Domain.Rates;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Infrastructure.Services;

/// <summary>
/// Qaraj borclarını yaradan servis
/// </summary>
internal sealed class GarageChargeGenerator(
    RateProvider rateProvider,
    ICachedPropertyOwnershipService cachedPropertyOwnershipService,
    IChargeRepository chargeRepository,
    ILogger<GarageChargeGenerator> logger)
{
    public async Task<(int Created, int SkippedNoRate)> GenerateChargesAsync(
        string period,
        DateTimeOffset issuedOn,
        HashSet<Guid> billedPropertyIds,
        List<Charge> createdCharges,
        HashSet<Guid> affectedOwnerIds,
        CancellationToken cancellationToken)
    {
        var garages = await cachedPropertyOwnershipService
            .GetByPropertyTypeAsync(PropertyType.Garage, cancellationToken);

        // Qaraj tariflərini cache et (tip üzrə)
        var garageRateCache = new Dictionary<string, RateResponse?>();

        int chargesCreated = 0;
        int skippedNoRate = 0;

        foreach (var ownership in garages)
        {
            Guid garageId = ownership.GarageId!.Value;
            GarageType? garageType = ownership.Garage!.GarageType;

            if (billedPropertyIds.Contains(garageId))
            {
                logger.LogDebug("Charge already exists for garage {GarageId} in period {Period}",
                    garageId, period);
                continue;
            }

            // Tarifi cache-dən al və ya DB-dən yüklə
            string cacheKey = garageType?.ToString() ?? "default";
            if (!garageRateCache.TryGetValue(cacheKey, out var garageRateResponse))
            {
                garageRateResponse = await rateProvider.GetGarageRateAtDateAsync(
                    garageType, issuedOn, cancellationToken);
                garageRateCache[cacheKey] = garageRateResponse;
            }

            if (garageRateResponse is null)
            {
                skippedNoRate++;
                logger.LogWarning(
                    "No active FixedGarage rate found for garage type {GarageType} (garage {GarageId}); skipping",
                    garageType, garageId);
                continue;
            }

            Charge charge = Charge.CreateForGarage(
                ownership.OwnerId,
                garageId,
                period,
                issuedOn,
                garageRateResponse.Amount,
                garageRateResponse.Amount,
                RateType.FixedGarage);

            chargeRepository.Add(charge);
            createdCharges.Add(charge);
            affectedOwnerIds.Add(ownership.OwnerId);
            chargesCreated++;

            logger.LogDebug("Created charge for garage {GarageId} (type {GarageType}), owner {OwnerId}, amount {Amount}",
                garageId, garageType, ownership.OwnerId, garageRateResponse.Amount);
        }

        return (chargesCreated, skippedNoRate);
    }
}
