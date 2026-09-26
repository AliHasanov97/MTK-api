using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Common.Presentation.Responses;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.Apartments.Queries.SearchApartments;

internal sealed class SearchApartmentsQueryHandler
    : IQueryHandler<SearchApartmentsQuery, SearchApartmentsResponse>
{
    private readonly IApartmentRepository _apartmentRepository;

    public SearchApartmentsQueryHandler(IApartmentRepository apartmentRepository)
    {
        _apartmentRepository = apartmentRepository;
    }

    public async Task<Result<SearchApartmentsResponse>> Handle(
        SearchApartmentsQuery request,
        CancellationToken cancellationToken)
    {
        var apartments = await _apartmentRepository.SearchAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalCount = await _apartmentRepository.CountAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            cancellationToken);

        var items = apartments.Select(apartment =>
        {
            var building = ResponseObjectWithName.Create(apartment.BuildingId, apartment.Building.Name);
            var currentOwner = apartment.CurrentOwnerId.HasValue && apartment.CurrentOwner != null
                ? ResponseObjectWithName.Create(apartment.CurrentOwnerId.Value, apartment.CurrentOwner.FullName)
                : null;

            return new ApartmentListItem(
                apartment.Id,
                building,
                apartment.ApartmentNumber,
                apartment.Floor,
                apartment.AreaSquareMeters,
                apartment.RoomCount,
                apartment.Status.ToString(),
                currentOwner);
        }).ToList();

        var response = new SearchApartmentsResponse(
            items,
            totalCount,
            request.Page ?? 1,
            request.PageSize ?? 10);

        return Result.Success(response);
    }
}
