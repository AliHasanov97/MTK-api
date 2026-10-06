using AutoMapper;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Warehouse.Domain.Nomenclatures;

namespace MTK.Modules.Warehouse.Application.Nomenclatures.GetAllNomenclatures;

internal sealed class GetAllNomenclaturesQueryHandler
    : IQueryHandler<GetAllNomenclaturesQuery, Result<List<NomenclatureDto>>>
{
    private readonly INomenclatureRepository _nomenclatureRepository;
    private readonly IMapper _mapper;

    public GetAllNomenclaturesQueryHandler(
        INomenclatureRepository nomenclatureRepository,
        IMapper mapper)
    {
        _nomenclatureRepository = nomenclatureRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<NomenclatureDto>>> Handle(
        GetAllNomenclaturesQuery request,
        CancellationToken cancellationToken)
    {
        var nomenclatures = await _nomenclatureRepository.GetAllAsync(cancellationToken);

        // Filter by IsActive if provided
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

        var dtos = _mapper.Map<List<NomenclatureDto>>(pagedNomenclatures);

        return Result.Success(dtos);
    }
}
