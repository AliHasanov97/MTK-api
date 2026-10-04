using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payments.Services;

/// <summary>One record's resolved display info — null when the referenced party/property
/// shadow hasn't synced yet (never blocks the response, just leaves a gap).</summary>
public sealed record PartyPropertyDisplayInfo(string? PartyName, string? PropertyLabel);

/// <summary>
/// Shared batch resolver behind IPaymentDisplayEnricher/IChargeDisplayEnricher — both
/// Payment and Charge carry the same (OwnerId?, VendorId?, ApartmentId?, GarageId?)
/// shape, so this does the actual Owner/Vendor/Apartment/Garage shadow lookups once,
/// batched by distinct id to avoid N+1 across a list/search result.
/// </summary>
internal static class PartyPropertyDisplayResolver
{
    public static async Task<Dictionary<Guid, PartyPropertyDisplayInfo>> ResolveAsync<T>(
        IReadOnlyCollection<T> items,
        Func<T, Guid> id,
        Func<T, Guid?> ownerId,
        Func<T, Guid?> vendorId,
        Func<T, Guid?> apartmentId,
        Func<T, Guid?> garageId,
        IOwnerRepository ownerRepository,
        IVendorRepository vendorRepository,
        IApartmentRepository apartmentRepository,
        IGarageRepository garageRepository,
        CancellationToken cancellationToken)
    {
        var ownerIds = items.Select(ownerId).Where(x => x.HasValue).Select(x => x!.Value).Distinct().ToList();
        var vendorIds = items.Select(vendorId).Where(x => x.HasValue).Select(x => x!.Value).Distinct().ToList();
        var apartmentIds = items.Select(apartmentId).Where(x => x.HasValue).Select(x => x!.Value).Distinct().ToList();
        var garageIds = items.Select(garageId).Where(x => x.HasValue).Select(x => x!.Value).Distinct().ToList();

        var ownerNames = ownerIds.Count > 0
            ? (await ownerRepository.ListFromIdsAsync(ownerIds, cancellationToken)).ToDictionary(o => o.Id, o => o.FullName)
            : new Dictionary<Guid, string>();
        var vendorNames = vendorIds.Count > 0
            ? (await vendorRepository.ListFromIdsAsync(vendorIds, cancellationToken)).ToDictionary(v => v.Id, v => v.Name)
            : new Dictionary<Guid, string>();
        var apartmentLabels = apartmentIds.Count > 0
            ? (await apartmentRepository.ListFromIdsWithBuildingAsync(apartmentIds, cancellationToken))
                .ToDictionary(a => a.Id, a => $"Mənzil {a.ApartmentNumber} — {a.Building.Name}")
            : new Dictionary<Guid, string>();
        var garageLabels = garageIds.Count > 0
            ? (await garageRepository.ListFromIdsAsync(garageIds, cancellationToken))
                .ToDictionary(g => g.Id, g => $"Qaraj {g.GarageNumber}")
            : new Dictionary<Guid, string>();

        var result = new Dictionary<Guid, PartyPropertyDisplayInfo>();
        foreach (var item in items)
        {
            string? name = ownerId(item) is { } oid
                ? ownerNames.GetValueOrDefault(oid)
                : vendorId(item) is { } vid
                    ? vendorNames.GetValueOrDefault(vid)
                    : null;

            string? label = apartmentId(item) is { } aid
                ? apartmentLabels.GetValueOrDefault(aid)
                : garageId(item) is { } gid
                    ? garageLabels.GetValueOrDefault(gid)
                    : null;

            result[id(item)] = new PartyPropertyDisplayInfo(name, label);
        }

        return result;
    }
}
