using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Warehouse.Domain.Nomenclatures;

namespace MTK.Modules.Warehouse.Application.Nomenclatures.Queries.GetNomenclatureById;

internal sealed class GetNomenclatureByIdQueryHandler
    : IQueryHandler<GetNomenclatureByIdQuery, NomenclatureResponse>
{
    private readonly INomenclatureRepository _nomenclatureRepository;
    private readonly IMapper _mapper;

    public GetNomenclatureByIdQueryHandler(
        INomenclatureRepository nomenclatureRepository,
        IMapper mapper)
    {
        _nomenclatureRepository = nomenclatureRepository;
        _mapper = mapper;
    }

    public async Task<Result<NomenclatureResponse>> Handle(
        GetNomenclatureByIdQuery request,
        CancellationToken cancellationToken)
    {
        var nomenclature = await _nomenclatureRepository.GetByIdAsync(request.Id, cancellationToken);

        if (nomenclature is null)
        {
            return Result.Failure<NomenclatureResponse>(NomenclatureErrors.NotFound(request.Id));
        }

        var response = _mapper.Map<NomenclatureResponse>(nomenclature);

        return Result.Success(response);
    }
}
