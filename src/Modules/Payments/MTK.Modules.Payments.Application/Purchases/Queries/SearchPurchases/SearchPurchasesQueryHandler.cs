using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Purchases.Queries.SearchPurchases;

internal sealed class SearchPurchasesQueryHandler
    : IQueryHandler<SearchPurchasesQuery, SearchPurchasesResponse>
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IVendorRepository _vendorRepository;

    public SearchPurchasesQueryHandler(
        IPurchaseRepository purchaseRepository,
        IVendorRepository vendorRepository)
    {
        _purchaseRepository = purchaseRepository;
        _vendorRepository = vendorRepository;
    }

    public async Task<Result<SearchPurchasesResponse>> Handle(
        SearchPurchasesQuery request,
        CancellationToken cancellationToken)
    {
        var purchases = await _purchaseRepository.SearchAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalCount = await _purchaseRepository.CountAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            cancellationToken);

        // Vendor adları ayrı aggregate-dən gəlir — sətir-sətir sorğu yerinə bir dəfə.
        var vendorIds = purchases.Select(p => p.VendorId).Distinct().ToList();
        var vendorNames = (await _vendorRepository.ListFromIdsAsync(vendorIds, cancellationToken))
            .ToDictionary(v => v.Id, v => v.Name);

        var items = purchases
            .Select(p => new PurchaseListItem(
                p.Id,
                p.VendorId,
                vendorNames.GetValueOrDefault(p.VendorId),
                p.PurchaseDate,
                p.InvoiceNumber,
                p.Status,
                p.TotalAmount,
                p.ReceivedOnUtc,
                p.CreatedAt))
            .ToList();

        var response = new SearchPurchasesResponse(
            items,
            totalCount,
            request.Page ?? 1,
            request.PageSize ?? 10);

        return response;
    }
}
