namespace MTK.Modules.Buildings.Application.Owners.Queries.GetOwnerById;

public sealed record OwnerResponse(
    Guid Id,
    Guid UserId,
    string FullName,
    string Email,
    string PhoneNumber,
    bool IsActive,
    int TotalApartments);
