using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Customers.Queries.GetCustomerById;

internal sealed class GetCustomerByIdQueryHandler : IQueryHandler<GetCustomerByIdQuery, CustomerResponse>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public GetCustomerByIdQueryHandler(ICustomerRepository customerRepository, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<Result<CustomerResponse>> Handle(
        GetCustomerByIdQuery request,
        CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);

        if (customer is null)
        {
            return Result.Failure<CustomerResponse>(new Error(
                "Customer.NotFound",
                $"Müştəri tapılmadı: {request.CustomerId}"));
        }

        return _mapper.Map<CustomerResponse>(customer);
    }
}
