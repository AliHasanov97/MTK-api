using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Customers;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Customers.Commands.CreateCustomer;

internal sealed class CreateCustomerCommandHandler : ICommandHandler<CreateCustomerCommand, Guid>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.Voen) &&
            !await _customerRepository.IsVoenUniqueAsync(request.Voen, null, cancellationToken))
        {
            return Result.Failure<Guid>(new Error(
                "Customer.VoenAlreadyExists",
                $"Bu VÖEN ilə müştəri artıq mövcuddur: {request.Voen}"));
        }

        var customer = Customer.Create(
            request.Name,
            request.CustomerType,
            request.Voen,
            request.Director,
            request.Email,
            request.Phone,
            request.Address,
            request.Note);

        _customerRepository.Add(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(customer.Id);
    }
}
