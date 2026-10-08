using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Purchases;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Purchases.Queries.GetNomenclaturePurchaseHistory;

internal sealed class GetNomenclaturePurchaseHistoryQueryHandler
    : IQueryHandler<GetNomenclaturePurchaseHistoryQuery, List<NomenclaturePurchaseHistoryItem>>
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IVendorRepository _vendorRepository;

    public GetNomenclaturePurchaseHistoryQueryHandler(
        IPurchaseRepository purchaseRepository,
        IVendorRepository vendorRepository)
    {
        _purchaseRepository = purchaseRepository;
        _vendorRepository = vendorRepository;
    }

    public async Task<Result<List<NomenclaturePurchaseHistoryItem>>> Handle(
        GetNomenclaturePurchaseHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var purchases = await _purchaseRepository.GetReceivedByNomenclatureIdAsync(
            request.NomenclatureId,
            cancellationToken);
        var vendorIds = purchases.Select(p => p.VendorId).Distinct().ToList();
        var vendors = await _vendorRepository.ListFromIdsAsync(vendorIds, cancellationToken);
        var vendorNames = vendors.ToDictionary(v => v.Id, v => v.Name);

        var result = purchases
            .SelectMany(p => p.Lines
                .Where(line => line.NomenclatureId == request.NomenclatureId)
                .Select(line => new NomenclaturePurchaseHistoryItem(
                    p.Id,
                    p.PurchaseDate,
                    p.InvoiceNumber,
                    vendorNames.GetValueOrDefault(p.VendorId),
                    p.VendorId,
                    (int)PurchaseStatus.Received,
                    line.Quantity,
                    line.UnitPrice,
                    line.LineTotal)))
            .ToList();

        return Result.Success(result);
    }
}
