using MTK.Common.Domain.Abstractions;
using MTK.Modules.Warehouse.Domain.Nomenclatures;

namespace MTK.Modules.Warehouse.Application.Nomenclatures.UpdateNomenclature;

internal sealed class UpdateNomenclatureCommandHandler : ICommandHandler<UpdateNomenclatureCommand, Result>
{
    private readonly INomenclatureRepository _nomenclatureRepository;

    public UpdateNomenclatureCommandHandler(INomenclatureRepository nomenclatureRepository)
    {
        _nomenclatureRepository = nomenclatureRepository;
    }

    public async Task<Result> Handle(UpdateNomenclatureCommand request, CancellationToken cancellationToken)
    {
        var nomenclature = await _nomenclatureRepository.GetByIdAsync(request.Id, cancellationToken);

        if (nomenclature is null)
        {
            return Result.Failure(NomenclatureErrors.NotFound(request.Id));
        }

        nomenclature.Update(
            request.Name,
            request.Description,
            request.Category,
            request.Unit,
            request.MinStockLevel);

        await _nomenclatureRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
