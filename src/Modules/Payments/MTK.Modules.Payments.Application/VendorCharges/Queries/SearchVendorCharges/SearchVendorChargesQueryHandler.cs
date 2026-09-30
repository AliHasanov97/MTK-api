using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.VendorCharges.Queries.SearchVendorCharges;

internal sealed class SearchVendorChargesQueryHandler
    : IQueryHandler<SearchVendorChargesQuery, SearchVendorChargesResponse>
{
    private readonly IVendorChargeRepository _vendorChargeRepository;

    public SearchVendorChargesQueryHandler(IVendorChargeRepository vendorChargeRepository)
    {
        _vendorChargeRepository = vendorChargeRepository;
    }

    public async Task<Result<SearchVendorChargesResponse>> Handle(
        SearchVendorChargesQuery request,
        CancellationToken cancellationToken)
    {
        var charges = await _vendorChargeRepository.SearchAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalCount = await _vendorChargeRepository.CountAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            cancellationToken);

        var items = charges.Select(ToResponse).ToList();

        return new SearchVendorChargesResponse(
            items,
            totalCount,
            request.Page ?? 1,
            request.PageSize ?? 10);
    }

    internal static VendorChargeResponse ToResponse(Domain.Charges.Charge c) => new(
        c.Id,
        c.ContractId,
        c.PartyId,
        c.ContractServiceId,
        c.Period,
        c.Description,
        c.Amount,
        c.PaidAmount,
        c.OutstandingAmount,
        c.Status,
        c.IssuedOn,
        c.DueDate,
        c.IsOverdue,
        c.CreatedAt);
}
