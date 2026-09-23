namespace MTK.Modules.Buildings.Application.Owners.Queries.GetOwnerById;

public sealed record OwnerResponse(
    Guid Id,
    Guid? UserId, // Nullable - passive owners don't have user accounts
    string FullName,
    string Email,
    string PhoneNumber,
    bool IsActive,
    int TotalApartments);
