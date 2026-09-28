using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Charges.Queries.SearchCharges;

internal sealed class SearchChargesQueryHandler : IQueryHandler<SearchChargesQuery, SearchChargesResponse>
{
    private readonly IChargeRepository _chargeRepository;

    public SearchChargesQueryHandler(IChargeRepository chargeRepository)
    {
        _chargeRepository = chargeRepository;
    }

    public async Task<Result<SearchChargesResponse>> Handle(
        SearchChargesQuery request,
        CancellationToken cancellationToken)
    {
        var charges = await _chargeRepository.SearchAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalCount = await _chargeRepository.CountAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            cancellationToken);

        var items = charges.Select(c => new ChargeSearchResult(
            c.Id,
            c.OwnerId,
            c.PropertyType,
            c.PropertyId,
            c.Period,
            c.Amount,
            c.PaidAmount,
            c.Status,
            c.CreatedAt)).ToList();

        var response = new SearchChargesResponse(
            items,
            totalCount,
            request.Page ?? 1,
            request.PageSize ?? 10);

        return response;
    }
}
