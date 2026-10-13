using Microsoft.Extensions.Logging;
using MTK.Modules.Payments.Application.PropertyOwnerships;
using MTK.Modules.Payments.Application.PropertyOwnerships.Services;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.PropertyOwnerships;
using MTK.Modules.Payments.Domain.Rates;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Infrastructure.Services;

/// <summary>
/// Mənzil borclarını yaradan servis
/// </summary>
internal sealed class ApartmentChargeGenerator(
    RateProvider rateProvider,
    ICachedPropertyOwnershipService cachedPropertyOwnershipService,
    IChargeRepository chargeRepository,
    ILogger<ApartmentChargeGenerator> logger)
{
    public async Task<(int Created, int SkippedNoRate)> GenerateChargesAsync(
        string period,
        DateTimeOffset issuedOn,
        HashSet<Guid> billedPropertyIds,
        List<Charge> createdCharges,
        HashSet<Guid> affectedOwnerIds,
        CancellationToken cancellationToken)
    {
        // Mənzil tarifini al
        var apartmentRateResponse = await rateProvider.GetRateAtDateAsync(
            RateType.PerSquareMeter, issuedOn, cancellationToken);

        var apartments = await cachedPropertyOwnershipService
            .GetByPropertyTypeAsync(PropertyType.Apartment, cancellationToken);

        if (apartmentRateResponse is null)
        {
            int skipped = apartments.Count(o => !billedPropertyIds.Contains(o.ApartmentId!.Value));
            if (skipped > 0)
            {
                logger.LogWarning(
                    "No active PerSquareMeter rate found for period {Period}; {Count} apartment(s) were NOT charged",
                    period, skipped);
            }
            return (0, skipped);
        }

        int chargesCreated = 0;

        foreach (var ownership in apartments)
        {
            Guid apartmentId = ownership.ApartmentId!.Value;

            if (billedPropertyIds.Contains(apartmentId))
            {
                logger.LogDebug("Charge already exists for apartment {ApartmentId} in period {Period}",
                    apartmentId, period);
                continue;
            }

            decimal area = ownership.Apartment!.AreaSquareMeters;
            decimal chargeAmount = apartmentRateResponse.Amount * area;

            Charge charge = Charge.CreateForApartment(
                ownership.OwnerId,
                apartmentId,
                period,
                issuedOn,
                chargeAmount,
                apartmentRateResponse.Amount,
                RateType.PerSquareMeter,
                area);

            chargeRepository.Add(charge);
            createdCharges.Add(charge);
            affectedOwnerIds.Add(ownership.OwnerId);
            chargesCreated++;

            logger.LogDebug("Created charge for apartment {ApartmentId}, owner {OwnerId}, amount {Amount}",
                apartmentId, ownership.OwnerId, chargeAmount);
        }

        return (chargesCreated, 0);
    }
}
