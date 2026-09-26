using MTK.Common.Presentation.Responses;

namespace MTK.Modules.Buildings.Application.Apartments.Queries.SearchApartments;

public sealed record SearchApartmentsResponse(
    List<ApartmentListItem> Items,
    int TotalCount,
    int Page,
    int PageSize);

public sealed record ApartmentListItem(
    Guid Id,
    ResponseObjectWithName Building,
    string ApartmentNumber,
    int Floor,
    decimal AreaSquareMeters,
    int RoomCount,
    string Status,
    ResponseObjectWithName? CurrentOwner);
