using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Payments.Services;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payments.Queries.SearchPayments;

internal sealed class SearchPaymentsQueryHandler : IQueryHandler<SearchPaymentsQuery, SearchPaymentsResponse>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentDisplayEnricher _displayEnricher;
    private readonly IMapper _mapper;

    public SearchPaymentsQueryHandler(
        IPaymentRepository paymentRepository,
        IPaymentDisplayEnricher displayEnricher,
        IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _displayEnricher = displayEnricher;
        _mapper = mapper;
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

        var display = await _displayEnricher.ResolveAsync(payments, cancellationToken);

        var items = _mapper.Map<List<PaymentSearchResult>>(payments)
            .Select(r => r with
            {
                PartyName = display.GetValueOrDefault(r.Id)?.PartyName,
                PropertyLabel = display.GetValueOrDefault(r.Id)?.PropertyLabel,
            })
            .ToList();

        var response = new SearchPaymentsResponse(
            items,
            totalCount,
            request.Page ?? 1,
            request.PageSize ?? 10);

        return response;
    }
}
