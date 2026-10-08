using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Purchases.Commands.UpdatePurchase;

internal sealed class UpdatePurchaseCommandHandler : ICommandHandler<UpdatePurchaseCommand>
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly INomenclatureShadowRepository _nomenclatureRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePurchaseCommandHandler(
        IPurchaseRepository purchaseRepository,
        INomenclatureShadowRepository nomenclatureRepository,
        IUnitOfWork unitOfWork)
    {
        _purchaseRepository = purchaseRepository;
        _nomenclatureRepository = nomenclatureRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdatePurchaseCommand request, CancellationToken cancellationToken)
    {
        var purchase = await _purchaseRepository.GetWithLinesAsync(request.PurchaseId, cancellationToken);
        if (purchase is null)
        {
            return Result.Failure(new Error(
                "Purchase.NotFound",
                $"Alış tapılmadı: {request.PurchaseId}"));
        }

        if (!purchase.IsEditable)
        {
            return Result.Failure(new Error(
                "Purchase.NotEditable",
                "Yalnız hazırlanan (Draft) alış dəyişdirilə bilər"));
        }

        if (request.Lines.Count == 0)
        {
            return Result.Failure(new Error(
                "Purchase.NoLines",
                "Alışın ən azı bir sətri olmalıdır"));
        }

        var nomenclatureIds = request.Lines.Select(l => l.NomenclatureId).Distinct().ToList();
        var found = await _nomenclatureRepository.ListFromIdsAsync(nomenclatureIds, cancellationToken);
        var foundIds = found.Select(n => n.Id).ToHashSet();
        var missing = nomenclatureIds.Where(id => !foundIds.Contains(id)).ToList();
        if (missing.Count > 0)
        {
            return Result.Failure(new Error(
                "Purchase.NomenclatureNotFound",
                $"Nomenklatura tapılmadı: {string.Join(", ", missing)}"));
        }

        purchase.Update(request.PurchaseDate, request.Note);

        // Sətirləri tam əvəz et — köhnələri çıxar, yenilərini əlavə et.
        foreach (var existingLine in purchase.Lines.ToList())
        {
            purchase.RemoveLine(existingLine.Id);
        }

        foreach (var line in request.Lines)
        {
            purchase.AddLine(line.NomenclatureId, line.Quantity, line.UnitPrice);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
