using MTK.Common.Domain.Abstractions;
using MTK.Modules.Warehouse.Domain.Nomenclatures;

namespace MTK.Modules.Warehouse.Application.Nomenclatures.CreateNomenclature;

internal sealed class CreateNomenclatureCommandHandler : ICommandHandler<CreateNomenclatureCommand, Result<Guid>>
{
    private readonly INomenclatureRepository _nomenclatureRepository;

    public CreateNomenclatureCommandHandler(INomenclatureRepository nomenclatureRepository)
    {
        _nomenclatureRepository = nomenclatureRepository;
    }

    public async Task<Result<Guid>> Handle(CreateNomenclatureCommand request, CancellationToken cancellationToken)
    {
        // Kod unique olmalıdır
        bool codeExists = await _nomenclatureRepository.IsCodeExistsAsync(request.Code, cancellationToken);
        if (codeExists)
        {
            return Result.Failure<Guid>(NomenclatureErrors.DuplicateCode(request.Code));
        }

        // Nomenclature yaradırıq
        var nomenclature = Nomenclature.Create(
            request.Code,
            request.Name,
            request.Description,
            request.Category,
            request.Unit,
            request.MinStockLevel,
            request.IsActive);

        _nomenclatureRepository.Add(nomenclature);

        await _nomenclatureRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(nomenclature.Id);
    }
}
