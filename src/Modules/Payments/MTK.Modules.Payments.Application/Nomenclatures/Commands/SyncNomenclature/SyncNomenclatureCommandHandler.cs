using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Nomenclatures;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Nomenclatures.Commands.SyncNomenclature;

internal sealed class SyncNomenclatureCommandHandler : ICommandHandler<SyncNomenclatureCommand>
{
    private readonly INomenclatureShadowRepository _nomenclatureRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SyncNomenclatureCommandHandler(
        INomenclatureShadowRepository nomenclatureRepository,
        IUnitOfWork unitOfWork)
    {
        _nomenclatureRepository = nomenclatureRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SyncNomenclatureCommand request, CancellationToken cancellationToken)
    {
        var nomenclature = await _nomenclatureRepository.GetByIdDefaultAsync(request.NomenclatureId, cancellationToken);

        if (nomenclature is null)
        {
            nomenclature = NomenclatureShadow.Create(
                request.NomenclatureId,
                request.Code,
                request.Name,
                request.Description,
                request.Category,
                request.Unit,
                request.MinStockLevel,
                request.IsActive);

            _nomenclatureRepository.Add(nomenclature);
        }
        else
        {
            nomenclature.Sync(
                request.Code,
                request.Name,
                request.Description,
                request.Category,
                request.Unit,
                request.MinStockLevel,
                request.IsActive);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
