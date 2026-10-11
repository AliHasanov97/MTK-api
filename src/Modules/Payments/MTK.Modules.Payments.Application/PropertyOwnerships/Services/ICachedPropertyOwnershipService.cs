using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.PropertyOwnerships;

namespace MTK.Modules.Payments.Application.PropertyOwnerships.Services;

/// <summary>
/// Redis-cached service for property ownership queries.
/// Returns response DTOs instead of domain entities with navigation properties.
/// Cache invalidation happens when apartments/garages/owners are created or updated.
/// </summary>
public interface ICachedPropertyOwnershipService
{
    /// <summary>
    /// Gets all property ownerships by property type (Apartment or Garage).
    /// Cached for 10 minutes (property ownership changes infrequently).
    /// Used primarily by ChargeGenerationService for monthly charge creation.
    /// </summary>
    /// <param name="propertyType">Type of property (Apartment or Garage)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of property ownership responses with essential data</returns>
    Task<List<PropertyOwnershipResponse>> GetByPropertyTypeAsync(
        PropertyType propertyType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates all property ownership cache entries.
    /// Call this when apartments, garages, or owners are created/updated/deleted.
    /// </summary>
    Task InvalidateCacheAsync(CancellationToken cancellationToken = default);
}
