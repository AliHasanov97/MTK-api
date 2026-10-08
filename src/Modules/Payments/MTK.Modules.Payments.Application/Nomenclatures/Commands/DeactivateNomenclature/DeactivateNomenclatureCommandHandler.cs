using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Nomenclatures.Commands.DeactivateNomenclature;

internal sealed class DeactivateNomenclatureCommandHandler : ICommandHandler<DeactivateNomenclatureCommand>
{
    private readonly INomenclatureShadowRepository _nomenclatureRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateNomenclatureCommandHandler(
        INomenclatureShadowRepository nomenclatureRepository,
        IUnitOfWork unitOfWork)
    {
        _nomenclatureRepository = nomenclatureRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeactivateNomenclatureCommand request, CancellationToken cancellationToken)
    {
        var nomenclature = await _nomenclatureRepository.GetByIdDefaultAsync(request.NomenclatureId, cancellationToken);
        if (nomenclature is null)
        {
            // Güzgü hələ sync olunmayıbsa, silinməsinin elə bir təsiri yoxdur.
            return Result.Success();
        }

        nomenclature.Deactivate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
