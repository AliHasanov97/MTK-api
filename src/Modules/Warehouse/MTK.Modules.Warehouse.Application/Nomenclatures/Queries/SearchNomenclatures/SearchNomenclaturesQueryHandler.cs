using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Warehouse.Domain.Nomenclatures;

namespace MTK.Modules.Warehouse.Application.Nomenclatures.Queries.SearchNomenclatures;

internal sealed class SearchNomenclaturesQueryHandler
    : IQueryHandler<SearchNomenclaturesQuery, SearchNomenclaturesResponse>
{
    private readonly INomenclatureRepository _nomenclatureRepository;
    private readonly IMapper _mapper;

    public SearchNomenclaturesQueryHandler(
        INomenclatureRepository nomenclatureRepository,
        IMapper mapper)
    {
        _nomenclatureRepository = nomenclatureRepository;
        _mapper = mapper;
    }

    public async Task<Result<SearchNomenclaturesResponse>> Handle(
        SearchNomenclaturesQuery request,
        CancellationToken cancellationToken)
    {
        var nomenclatures = await _nomenclatureRepository.SearchAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalCount = await _nomenclatureRepository.CountAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            cancellationToken);

        var items = _mapper.Map<List<NomenclatureListItem>>(nomenclatures);

        var response = new SearchNomenclaturesResponse(
            items,
            totalCount,
            request.Page ?? 1,
            request.PageSize ?? 10);

        return response;
    }
}
