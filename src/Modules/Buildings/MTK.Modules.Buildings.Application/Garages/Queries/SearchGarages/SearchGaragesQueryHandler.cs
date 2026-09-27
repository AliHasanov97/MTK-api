using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Common.Presentation.Responses;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.Garages.Queries.SearchGarages;

internal sealed class SearchGaragesQueryHandler
    : IQueryHandler<SearchGaragesQuery, SearchGaragesResponse>
{
    private readonly IGarageRepository _garageRepository;

    public SearchGaragesQueryHandler(IGarageRepository garageRepository)
    {
        _garageRepository = garageRepository;
    }

    public async Task<Result<SearchGaragesResponse>> Handle(
        SearchGaragesQuery request,
        CancellationToken cancellationToken)
    {
        var garages = await _garageRepository.SearchAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalCount = await _garageRepository.CountAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            cancellationToken);

        var items = garages.Select(garage =>
        {
            var owner = garage.OwnerId.HasValue && garage.Owner != null
                ? ResponseObjectWithName.Create(garage.OwnerId.Value, garage.Owner.FullName)
                : null;

            return new GarageListItem(
                garage.Id,
                garage.GarageNumber,
                garage.Type.ToString(),
                garage.Description,
                owner);
        }).ToList();

        var response = new SearchGaragesResponse(
            items,
            totalCount,
            request.Page ?? 1,
            request.PageSize ?? 10);

        return Result.Success(response);
    }
}
