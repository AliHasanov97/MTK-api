using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Warehouse.Application.Abstractions.Data;
using MTK.Modules.Warehouse.Domain.Nomenclatures;

namespace MTK.Modules.Warehouse.Application.Nomenclatures.Commands.UpdateNomenclature;

internal sealed class UpdateNomenclatureCommandHandler : ICommandHandler<UpdateNomenclatureCommand>
{
    private readonly INomenclatureRepository _nomenclatureRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateNomenclatureCommandHandler(
        INomenclatureRepository nomenclatureRepository,
        IUnitOfWork unitOfWork)
    {
        _nomenclatureRepository = nomenclatureRepository;
        _unitOfWork = unitOfWork;
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

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
