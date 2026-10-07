using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Warehouse.Application.Abstractions.Data;
using MTK.Modules.Warehouse.Domain.Nomenclatures;

namespace MTK.Modules.Warehouse.Application.Nomenclatures.Commands.DeleteNomenclature;

internal sealed class DeleteNomenclatureCommandHandler : ICommandHandler<DeleteNomenclatureCommand>
{
    private readonly INomenclatureRepository _nomenclatureRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteNomenclatureCommandHandler(
        INomenclatureRepository nomenclatureRepository,
        IUnitOfWork unitOfWork)
    {
        _nomenclatureRepository = nomenclatureRepository;
        _unitOfWork = unitOfWork;
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

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
