using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Customers.Commands.DeleteCustomer;

internal sealed class DeleteCustomerCommandHandler : ICommandHandler<DeleteCustomerCommand>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCustomerCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);

        if (customer is null)
        {
            return Result.Failure(new Error("Customer.NotFound", $"Müştəri tapılmadı: {request.CustomerId}"));
        }

        if (customer.DeletedAt is not null)
        {
            return Result.Failure(new Error("Customer.AlreadyDeleted", "Müştəri artıq silinib"));
        }

        if (await _customerRepository.HasActiveContractsAsync(request.CustomerId, cancellationToken))
        {
            return Result.Failure(new Error(
                "Customer.HasActiveContracts",
                "Bu müştərinin aktiv müqaviləsi var. Əvvəlcə müqaviləni ləğv edin və ya müştərini dayandırın"));
        }

        if (await _customerRepository.HasOpenChargesAsync(request.CustomerId, cancellationToken))
        {
            return Result.Failure(new Error(
                "Customer.HasOpenCharges",
                "Bu müştərinin açıq borcu var. Əvvəlcə borcları bağlayın və ya ləğv edin"));
        }

        customer.Delete();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
