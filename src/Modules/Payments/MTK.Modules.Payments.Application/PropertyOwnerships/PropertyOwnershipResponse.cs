using MTK.Modules.Payments.Domain.PropertyOwnerships;

namespace MTK.Modules.Payments.Application.PropertyOwnerships;

/// <summary>
/// Cached response for PropertyOwnership with essential data for charge generation.
/// Includes flattened apartment/garage data to avoid deep object graphs in cache.
/// </summary>
public sealed record PropertyOwnershipResponse(
    Guid Id,
    Guid OwnerId,
    Guid? ApartmentId,
    Guid? GarageId,
    ApartmentData? Apartment,
    GarageData? Garage);

/// <summary>
/// Essential apartment data for charge calculation.
/// </summary>
public sealed record ApartmentData(
    decimal AreaSquareMeters,
    string ApartmentNumber,
    Guid BuildingId);

/// <summary>
/// Essential garage data for charge calculation.
/// </summary>
public sealed record GarageData(
    GarageType GarageType,
    string GarageNumber);
