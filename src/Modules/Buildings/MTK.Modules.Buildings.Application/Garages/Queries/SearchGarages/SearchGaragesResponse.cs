using MTK.Common.Presentation.Responses;

namespace MTK.Modules.Buildings.Application.Garages.Queries.SearchGarages;

public sealed record SearchGaragesResponse(
    List<GarageListItem> Items,
    int TotalCount,
    int Page,
    int PageSize);

public sealed record GarageListItem(
    Guid Id,
    string GarageNumber,
    string Type,
    string? Description,
    ResponseObjectWithName? Owner);
