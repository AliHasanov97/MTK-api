namespace MTK.Modules.Buildings.Application.Owners.Queries.SearchOwners;

public sealed record SearchOwnersResponse(
    List<OwnerListItem> Items,
    int TotalCount,
    int Page,
    int PageSize);

public sealed record OwnerListItem(
    Guid Id,
    Guid? UserId,
    string FullName,
    string Email,
    string PhoneNumber,
    bool IsActive);
