using MTK.Common.Domain.Abstractions;
using MTK.Modules.Warehouse.Domain.Nomenclatures;

namespace MTK.Modules.Warehouse.Application.Nomenclatures.DeleteNomenclature;

internal sealed class DeleteNomenclatureCommandHandler : ICommandHandler<DeleteNomenclatureCommand, Result>
{
    private readonly INomenclatureRepository _nomenclatureRepository;

    public DeleteNomenclatureCommandHandler(INomenclatureRepository nomenclatureRepository)
    {
        _nomenclatureRepository = nomenclatureRepository;
    }

    public async Task<Result> Handle(DeleteNomenclatureCommand request, CancellationToken cancellationToken)
    {
        var nomenclature = await _nomenclatureRepository.GetByIdAsync(request.Id, cancellationToken);

        if (nomenclature is null)
        {
            return Result.Failure(NomenclatureErrors.NotFound(request.Id));
        }

        if (nomenclature.DeletedAt.HasValue)
        {
            return Result.Failure(NomenclatureErrors.AlreadyDeleted);
        }

        // Soft delete
        nomenclature.Delete();

        await _nomenclatureRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
