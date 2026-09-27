using MTK.Common.Presentation.Responses;

namespace MTK.Modules.Buildings.Application.Owners.Queries.GetOwnerById;

public sealed record OwnerResponse(
    Guid Id,
    Guid? UserId, // Nullable - passive owners don't have user accounts
    string FullName,
    string Email,
    string PhoneNumber,
    bool IsActive,
    List<OwnedApartmentSummary> Apartments,
    List<OwnedGarageSummary> Garages);

public sealed record OwnedApartmentSummary(
    Guid Id,
    ResponseObjectWithName Building,
    string ApartmentNumber,
    int Floor,
    decimal AreaSquareMeters,
    int RoomCount,
    string Status);

public sealed record OwnedGarageSummary(
    Guid Id,
    string GarageNumber,
    string Type,
    string? Description);
