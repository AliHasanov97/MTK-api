using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Customers.Commands.UpdateCustomer;

internal sealed class UpdateCustomerCommandHandler : ICommandHandler<UpdateCustomerCommand>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);

        if (customer is null)
        {
            return Result.Failure(new Error("Customer.NotFound", $"Müştəri tapılmadı: {request.CustomerId}"));
        }

        if (!string.IsNullOrWhiteSpace(request.Voen) &&
            !await _customerRepository.IsVoenUniqueAsync(request.Voen, request.CustomerId, cancellationToken))
        {
            return Result.Failure(new Error(
                "Customer.VoenAlreadyExists",
                $"Bu VÖEN ilə başqa müştəri mövcuddur: {request.Voen}"));
        }

        customer.UpdateIdentity(request.Name, request.CustomerType, request.Voen);
        customer.UpdateContactInfo(
            request.Director,
            request.Email,
            request.Phone,
            request.Address,
            request.Note);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
