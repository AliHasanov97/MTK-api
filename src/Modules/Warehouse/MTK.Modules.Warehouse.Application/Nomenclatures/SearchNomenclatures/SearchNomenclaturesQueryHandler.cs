using AutoMapper;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Warehouse.Domain.Nomenclatures;

namespace MTK.Modules.Warehouse.Application.Nomenclatures.SearchNomenclatures;

internal sealed class SearchNomenclaturesQueryHandler
    : IQueryHandler<SearchNomenclaturesQuery, Result<List<NomenclatureSearchDto>>>
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

    public async Task<Result<List<NomenclatureSearchDto>>> Handle(
        SearchNomenclaturesQuery request,
        CancellationToken cancellationToken)
    {
        var nomenclatures = await _nomenclatureRepository.GetAllAsync(cancellationToken);

        // Search by term (code or name)
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTermLower = request.SearchTerm.ToLowerInvariant();
            nomenclatures = nomenclatures
                .Where(n =>
                    n.Code.ToLowerInvariant().Contains(searchTermLower) ||
                    n.Name.ToLowerInvariant().Contains(searchTermLower))
                .ToList();
        }

        // Filter by category
        if (request.Category.HasValue)
        {
            nomenclatures = nomenclatures
                .Where(n => n.Category == request.Category.Value)
                .ToList();
        }

        // Filter by IsActive
        if (request.IsActive.HasValue)
        {
            nomenclatures = nomenclatures
                .Where(n => n.IsActive == request.IsActive.Value)
                .ToList();
        }

        // Pagination
        var pagedNomenclatures = nomenclatures
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var dtos = _mapper.Map<List<NomenclatureSearchDto>>(pagedNomenclatures);

        return Result.Success(dtos);
    }
}
