using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payments.Queries.SearchPayments;

internal sealed class SearchPaymentsQueryHandler : IQueryHandler<SearchPaymentsQuery, SearchPaymentsResponse>
{
    private readonly IPaymentRepository _paymentRepository;

    public SearchPaymentsQueryHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<Result<SearchPaymentsResponse>> Handle(
        SearchPaymentsQuery request,
        CancellationToken cancellationToken)
    {
        var payments = await _paymentRepository.SearchAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalCount = await _paymentRepository.CountAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            cancellationToken);

        var items = payments.Select(p => new PaymentSearchResult(
            p.Id,
            p.PartyId,
            p.Amount,
            p.PaymentMethod,
            p.PaymentDate,
            p.Status,
            p.Notes,
            p.CreatedAt,
            p.PropertyId,
            p.PropertyType)).ToList();

        var response = new SearchPaymentsResponse(
            items,
            totalCount,
            request.Page ?? 1,
            request.PageSize ?? 10);

        return response;
    }
}
