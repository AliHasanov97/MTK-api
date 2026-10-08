using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Purchases;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Purchases.Commands.CreatePurchase;

internal sealed class CreatePurchaseCommandHandler : ICommandHandler<CreatePurchaseCommand, Guid>
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IVendorRepository _vendorRepository;
    private readonly INomenclatureShadowRepository _nomenclatureRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePurchaseCommandHandler(
        IPurchaseRepository purchaseRepository,
        IVendorRepository vendorRepository,
        INomenclatureShadowRepository nomenclatureRepository,
        IUnitOfWork unitOfWork)
    {
        _purchaseRepository = purchaseRepository;
        _vendorRepository = vendorRepository;
        _nomenclatureRepository = nomenclatureRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreatePurchaseCommand request, CancellationToken cancellationToken)
    {
        if (request.Lines.Count == 0)
        {
            return Result.Failure<Guid>(new Error(
                "Purchase.NoLines",
                "Alışın ən azı bir sətri olmalıdır"));
        }

        var vendor = await _vendorRepository.GetByIdAsync(request.VendorId, cancellationToken);
        if (vendor is null)
        {
            return Result.Failure<Guid>(new Error(
                "Purchase.VendorNotFound",
                $"Tədarükçü tapılmadı: {request.VendorId}"));
        }

        var nomenclatureError = await ValidateNomenclaturesAsync(request.Lines, cancellationToken);
        if (nomenclatureError is not null)
        {
            return Result.Failure<Guid>(nomenclatureError);
        }

        var purchase = Purchase.Create(
            request.VendorId,
            request.PurchaseDate,
            request.Note,
            request.CreatedByUserId);

        foreach (var line in request.Lines)
        {
            purchase.AddLine(line.NomenclatureId, line.Quantity, line.UnitPrice);
        }

        _purchaseRepository.Add(purchase);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(purchase.Id);
    }

    /// <summary>
    /// Nomenklaturalar yalnız Warehouse-dan sync olunur — göndərilən Id-lər bu modulun
    /// güzgüsündə olmalıdır (əks halda sətir "boş" istinad daşıyır).
    /// </summary>
    private async Task<Error?> ValidateNomenclaturesAsync(
        IReadOnlyCollection<PurchaseLineInput> lines,
        CancellationToken cancellationToken)
    {
        var nomenclatureIds = lines.Select(l => l.NomenclatureId).Distinct().ToList();
        var found = await _nomenclatureRepository.ListFromIdsAsync(nomenclatureIds, cancellationToken);
        var foundIds = found.Select(n => n.Id).ToHashSet();

        var missing = nomenclatureIds.Where(id => !foundIds.Contains(id)).ToList();
        if (missing.Count == 0)
        {
            return null;
        }

        return new Error(
            "Purchase.NomenclatureNotFound",
            $"Nomenklatura tapılmadı: {string.Join(", ", missing)}");
    }
}
