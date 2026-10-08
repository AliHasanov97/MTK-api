using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Nomenclatures.Queries.SearchNomenclatureShadows;

internal sealed class SearchNomenclatureShadowsQueryHandler
    : IQueryHandler<SearchNomenclatureShadowsQuery, SearchNomenclatureShadowsResponse>
{
    private readonly INomenclatureShadowRepository _nomenclatureRepository;
    private readonly IMapper _mapper;

    public SearchNomenclatureShadowsQueryHandler(
        INomenclatureShadowRepository nomenclatureRepository,
        IMapper mapper)
    {
        _nomenclatureRepository = nomenclatureRepository;
        _mapper = mapper;
    }

    public async Task<Result<SearchNomenclatureShadowsResponse>> Handle(
        SearchNomenclatureShadowsQuery request,
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

        var items = _mapper.Map<IReadOnlyCollection<NomenclatureShadowSearchResult>>(nomenclatures);

        var response = new SearchNomenclatureShadowsResponse(
            items,
            totalCount,
            request.Page ?? 1,
            request.PageSize ?? 10);

        return response;
    }
}
