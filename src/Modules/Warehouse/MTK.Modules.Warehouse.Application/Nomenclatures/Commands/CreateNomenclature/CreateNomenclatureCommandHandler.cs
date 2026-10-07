using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Warehouse.Application.Abstractions.Data;
using MTK.Modules.Warehouse.Domain.Nomenclatures;

namespace MTK.Modules.Warehouse.Application.Nomenclatures.Commands.CreateNomenclature;

internal sealed class CreateNomenclatureCommandHandler : ICommandHandler<CreateNomenclatureCommand, Guid>
{
    private readonly INomenclatureRepository _nomenclatureRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateNomenclatureCommandHandler(
        INomenclatureRepository nomenclatureRepository,
        IUnitOfWork unitOfWork)
    {
        _nomenclatureRepository = nomenclatureRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateNomenclatureCommand request, CancellationToken cancellationToken)
    {
        // Kod unique olmalıdır
        bool codeExists = await _nomenclatureRepository.IsCodeExistsAsync(request.Code, cancellationToken);
        if (codeExists)
        {
            return Result.Failure<Guid>(NomenclatureErrors.DuplicateCode(request.Code));
        }

        var nomenclature = Nomenclature.Create(
            request.Code,
            request.Name,
            request.Description,
            request.Category,
            request.Unit,
            request.MinStockLevel,
            request.IsActive);

        _nomenclatureRepository.Add(nomenclature);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(nomenclature.Id);
    }
}
