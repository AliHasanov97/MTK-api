using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Customers.Commands.SetCustomerStatus;

internal sealed class SetCustomerStatusCommandHandler : ICommandHandler<SetCustomerStatusCommand>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SetCustomerStatusCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SetCustomerStatusCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);

        if (customer is null)
        {
            return Result.Failure(new Error("Customer.NotFound", $"Müştəri tapılmadı: {request.CustomerId}"));
        }

        if (request.IsActive)
        {
            customer.Activate();
        }
        else
        {
            customer.Deactivate();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
