using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Parties;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Customers.Queries.GetCustomerBalance;

internal sealed class GetCustomerBalanceQueryHandler : IQueryHandler<GetCustomerBalanceQuery, CustomerBalanceResponse>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICustomerChargeRepository _customerChargeRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentAllocationRepository _paymentAllocationRepository;

    public GetCustomerBalanceQueryHandler(
        ICustomerRepository customerRepository,
        ICustomerChargeRepository customerChargeRepository,
        IPaymentRepository paymentRepository,
        IPaymentAllocationRepository paymentAllocationRepository)
    {
        _customerRepository = customerRepository;
        _customerChargeRepository = customerChargeRepository;
        _paymentRepository = paymentRepository;
        _paymentAllocationRepository = paymentAllocationRepository;
    }

    public async Task<Result<CustomerBalanceResponse>> Handle(
        GetCustomerBalanceQuery request,
        CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);

        if (customer is null)
        {
            return Result.Failure<CustomerBalanceResponse>(new Error(
                "Customer.NotFound",
                $"Müştəri tapılmadı: {request.CustomerId}"));
        }

        var charges = await _customerChargeRepository.ListByCustomerAsync(request.CustomerId, cancellationToken);
        decimal outstandingDebt = charges
            .Where(c => c.Status is not (ChargeStatus.Paid or ChargeStatus.Cancelled))
            .Sum(c => c.OutstandingAmount);

        var payments = await _paymentRepository.GetCompletedByPartyIdsAsync(
            PartyType.Customer, [request.CustomerId], cancellationToken);
        decimal totalPaid = payments.Sum(p => p.Amount);

        // Avans = ödənişlərin cəmi − həmin ödənişlərdən borclara artıq bölünmüş hissə.
        decimal allocated = 0;
        if (payments.Count > 0)
        {
            var allocations = await _paymentAllocationRepository.GetByPaymentIdsAsync(
                payments.Select(p => p.Id).ToList(), cancellationToken);
            allocated = allocations.Sum(a => a.Amount);
        }

        return new CustomerBalanceResponse(
            request.CustomerId,
            outstandingDebt,
            Math.Max(0, totalPaid - allocated),
            totalPaid);
    }
}
