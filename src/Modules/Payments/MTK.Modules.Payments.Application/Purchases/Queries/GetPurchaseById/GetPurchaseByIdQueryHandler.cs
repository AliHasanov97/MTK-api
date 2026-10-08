using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Purchases.Queries.GetPurchaseById;

internal sealed class GetPurchaseByIdQueryHandler : IQueryHandler<GetPurchaseByIdQuery, PurchaseResponse>
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IVendorRepository _vendorRepository;
    private readonly INomenclatureShadowRepository _nomenclatureRepository;

    public GetPurchaseByIdQueryHandler(
        IPurchaseRepository purchaseRepository,
        IVendorRepository vendorRepository,
        INomenclatureShadowRepository nomenclatureRepository)
    {
        _purchaseRepository = purchaseRepository;
        _vendorRepository = vendorRepository;
        _nomenclatureRepository = nomenclatureRepository;
    }

    public async Task<Result<PurchaseResponse>> Handle(
        GetPurchaseByIdQuery request,
        CancellationToken cancellationToken)
    {
        var purchase = await _purchaseRepository.GetWithLinesAsync(request.PurchaseId, cancellationToken);
        if (purchase is null)
        {
            return Result.Failure<PurchaseResponse>(new Error(
                "Purchase.NotFound",
                $"Alış tapılmadı: {request.PurchaseId}"));
        }

        var vendor = await _vendorRepository.GetByIdAsync(purchase.VendorId, cancellationToken);

        // Nomenklatura adları ayrı aggregate-dən gəlir — sətir-sətir sorğu yerinə bir dəfə.
        var nomenclatureIds = purchase.Lines.Select(l => l.NomenclatureId).Distinct().ToList();
        var nomenclatures = await _nomenclatureRepository.ListFromIdsAsync(nomenclatureIds, cancellationToken);
        var nomenclatureById = nomenclatures.ToDictionary(n => n.Id);

        var lines = purchase.Lines
            .Select(l =>
            {
                nomenclatureById.TryGetValue(l.NomenclatureId, out var nomenclature);
                return new PurchaseLineResponse(
                    l.Id,
                    l.NomenclatureId,
                    nomenclature?.Code,
                    nomenclature?.Name,
                    l.Quantity,
                    l.UnitPrice,
                    l.LineTotal);
            })
            .ToList();

        var response = new PurchaseResponse(
            purchase.Id,
            purchase.VendorId,
            vendor?.Name,
            purchase.PurchaseDate,
            purchase.InvoiceNumber,
            purchase.Note,
            purchase.Status,
            purchase.ReceivedOnUtc,
            purchase.TotalAmount,
            purchase.CreatedAt,
            purchase.UpdatedAt,
            lines);

        return response;
    }
}
